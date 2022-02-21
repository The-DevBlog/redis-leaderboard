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
        /// <returns>List<LeaderboardEntryModel></returns>
        public async Task<List<LeaderboardEntryModel>> GetLeaderboardEntries(int currentPg, int perPage)
        {
            if (numberOfEntries == 0)
                await LoadDB();

            return await GetFromRedisCache(currentPg, perPage);
        }

        /// <summary>
        /// Adds a leaderboard entry into the redis cache
        /// </summary>
        /// <param name="entry">New leaderboard entry</param>
        public async Task AddLeaderboardEntry(LeaderboardEntryModel entry)
        {
            await _db.SortedSetAddAsync("leaderboard", entry.username, entry.score);
        }

        /// <summary>
        /// Deletes a leaderboard entry from the redis cache
        /// </summary>
        /// <param name="username">Leaderboard entry to delete</param>
        public async Task DeleteEntry(string username)
        {
            await _db.SortedSetRemoveAsync("leaderboard", username);
        }

        /// <summary>
        /// Returns a list of LeaderboardEntryModels for the current page from the redis cache
        /// </summary>
        /// <param name="currentPg">Page number of the current page</param>
        /// <param name="perPage">Number of entries per page</param>
        /// <returns>List<LeaderboardEntryModel></returns>
        private async Task<List<LeaderboardEntryModel>> GetFromRedisCache(int currentPg = 1, int perPage = 10)
        {
            numberOfEntries = (int)await _db.SortedSetLengthAsync("leaderboard");

            // calculate the range to retrieve based on the current
            // page number and the number of entries per page
            int from = currentPg * perPage - perPage;
            int to = from + (perPage - 1);

            var redisData = await _db.SortedSetRangeByRankWithScoresAsync("leaderboard", from, to, Order.Descending);

            // convert redis data to List<LeaderboardEntryModel> and return
            return redisData.Select(obj => new LeaderboardEntryModel(obj.Element, (int)obj.Score)).ToList();
        }

        /// <summary>
        /// Restores the leaderboard data with the default JSON data
        /// </summary>
        /// <returns>List<LeaderboardEntryModel></returns>
        public async Task<List<LeaderboardEntryModel>> RestoreDefaultData()
        {
            await _db.SortedSetRemoveRangeByRankAsync("leaderboard", 0, -1);
            await LoadDB();
            return await GetFromRedisCache();
        }

        /// <summary>
        /// Returns a list of LeaderboardEntryModels from the DB (json data)
        /// </summary>
        /// <returns>List<LeaderboardEntryModel></returns>
        public async Task LoadDB()
        {
            // get starter data from JSON file
            StreamReader r = new StreamReader("Data/data.json");
            string json = r.ReadToEnd();
            var result = JsonSerializer.Deserialize<List<LeaderboardEntryModel>>(json);

            // add to redis sorted set
            foreach (var obj in result)
                await _db.SortedSetAddAsync("leaderboard", obj.username, obj.score);
        }
    }
}
