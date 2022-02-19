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

        public LeaderboardEntryService(IConfiguration config)
        {
            _config = config;
            _redis = ConnectionMultiplexer.Connect(_config.GetConnectionString("Redis"));
            _db = _redis.GetDatabase(0);
        }

        public async Task<List<LeaderboardEntryModel>> GetLeaderboardEntries(List<LeaderboardEntryModel> currentEntries)
        {
            var result = new List<LeaderboardEntryModel>();

            // if there are no current entries 
            if (currentEntries is null)
                result = await GetFromDB();
            // else, get data from redis cache
            else
            {
                var redisData = await _db.SortedSetRangeByScoreWithScoresAsync("leaderboard");

                // sort by descending score
                Array.Reverse(redisData);
                result = redisData.Select(obj => new LeaderboardEntryModel(obj.Element, (int)obj.Score)).ToList();
            }

            return result;
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
