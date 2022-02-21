using RedisLeaderboard.Interfaces;
using RedisLeaderboard.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace RedisLeaderboard.Services
{
    public class LeaderboardEntryService : ILeaderboardEntryService
    {
        ConnectionMultiplexer _redis;
        IDatabase _db;
        IConfiguration _config;
        private int numberOfEntries;

        public LeaderboardEntryService(IConfiguration config)
        {
            _config = config;
            _redis = ConnectionMultiplexer.Connect(_config.GetConnectionString("Redis"));
            _db = _redis.GetDatabase(0);
        }

        public LeaderboardEntryService(IConfiguration config, IDatabase db, ConnectionMultiplexer redis)
        {
            _config = config;
            _db = db;
            _redis = redis;
        }

        /// <summary>
        /// Returns the total number of leaderboard entries
        /// </summary>
        /// <returns>int</returns>
        public int GetTotalCount()
        {
            return numberOfEntries;
        }

        /// <summary>
        /// Returns a list of LeaderboardEntryModels for one page
        /// </summary>
        /// <param name="currentPg">Page number of the current page</param>
        /// <param name="perPage">Number of entries per page</param>
        /// <param name="db">Specified redis DB</param>
        /// <returns>List<LeaderboardEntryModel></returns>
        public async Task<List<LeaderboardEntryModel>> GetEntriesForPage(int currentPg, int perPage, string db = "leaderboard")
        {
            if (numberOfEntries == 0)
                await LoadDB(db);

            return await GetFromRedisCache(currentPg, perPage, db);
        }

        /// <summary>
        /// Adds a leaderboard entry into the redis cache
        /// </summary>
        /// <param name="entry">New leaderboard entry</param>
        /// <param name="db">Specified redis DB</param>
        public async Task AddEntry(LeaderboardEntryModel entry, string db = "leaderboard")
        {
            await _db.SortedSetAddAsync(db, entry.username, entry.score);
        }

        /// <summary>
        /// Deletes a leaderboard entry from the redis cache
        /// </summary>
        /// <param name="username">Leaderboard entry to delete</param>
        /// <param name="db">Specified redis DB</param>
        public async Task DeleteEntry(string username, string db = "leaderboard")
        {
            await _db.SortedSetRemoveAsync(db, username);
        }

        /// <summary>
        /// Returns a list of LeaderboardEntryModels for the current page from the redis cache
        /// </summary>
        /// <param name="currentPg">Page number of the current page</param>
        /// <param name="perPage">Number of entries per page</param>
        /// <param name="db">Specified redis DB</param>
        /// <returns>List<LeaderboardEntryModel></returns>
        private async Task<List<LeaderboardEntryModel>> GetFromRedisCache(int currentPg = 1, int perPage = 10, string db = "leaderboard")
        {
            numberOfEntries = (int)await _db.SortedSetLengthAsync(db);

            // calculate the range to retrieve based on the current
            // page number and the number of entries per page
            int from = currentPg * perPage - perPage;
            int to = from + (perPage - 1);

            var redisData = await _db.SortedSetRangeByRankWithScoresAsync(db, from, to, Order.Descending);

            // convert redis data to List<LeaderboardEntryModel> and return
            return redisData.Select(obj => new LeaderboardEntryModel(obj.Element, (int)obj.Score)).ToList();
        }

        /// <summary>
        /// Restores the leaderboard data with the default JSON data
        /// </summary>
        /// <param name="db">Specified redis DB</param>
        /// <returns>List<LeaderboardEntryModel></returns>
        public async Task<List<LeaderboardEntryModel>> RestoreDefaultData(string db = "leaderboard")
        {
            await _db.SortedSetRemoveRangeByRankAsync(db, 0, -1);
            await LoadDB(db);
            return await GetFromRedisCache();
        }

        /// <summary>
        /// Returns a list of LeaderboardEntryModels from the DB (json data)
        /// </summary>
        /// <param name="db">Specified redis DB</param>
        /// <returns>List<LeaderboardEntryModel></returns>
        public async Task LoadDB(string db = "leaderboard")
        {
            // get starter data from JSON file
            StreamReader r = new StreamReader("Data/data.json");
            string json = r.ReadToEnd();
            var result = JsonSerializer.Deserialize<List<LeaderboardEntryModel>>(json);

            // add to redis sorted set
            foreach (var obj in result)
                await _db.SortedSetAddAsync(db, obj.username, obj.score);
        }
    }
}
