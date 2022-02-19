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

        public int GetTotalCount()
        {
            return numberOfEntries;
        }

        public async Task<List<LeaderboardEntryModel>> GetLeaderboardEntries(List<LeaderboardEntryModel> currentEntries, int currentPg)
        {
            // if 0 current entries, get from DB (json file), else, get from redis cache
            var redisData = await GetFromRedisCache(currentPg);
            return redisData.Count == 0 ? await GetFromDB() : redisData;
        }

        public async Task AddLeaderboardEntry(LeaderboardEntryModel entry)
        {
            await _db.SortedSetAddAsync("leaderboard", entry.username, entry.score);
        }

        public async Task DeleteEntry(string username)
        {
            await _db.SortedSetRemoveAsync("leaderboard", username);
        }

        private async Task<List<LeaderboardEntryModel>> GetFromRedisCache(int currentPg)
        {
            numberOfEntries = (int)await _db.SortedSetLengthAsync("leaderboard");

            int from = currentPg * 10 - 10;
            int to = from + 9;

            var redisData = await _db.SortedSetRangeByRankWithScoresAsync("leaderboard", from, to, Order.Descending);

            return redisData.Select(obj => new LeaderboardEntryModel(obj.Element, (int)obj.Score)).ToList();
        }

        private async Task<List<LeaderboardEntryModel>> GetFromDB()
        {
            // get data from JSON file
            StreamReader r = new StreamReader("Data/data.json");
            string json = r.ReadToEnd();
            var result = JsonSerializer.Deserialize<List<LeaderboardEntryModel>>(json);

            // add to redis sorted set
            foreach (var obj in result)
                await _db.SortedSetAddAsync("leaderboard", obj.username, obj.score);

            return result;
        }
    }
}
