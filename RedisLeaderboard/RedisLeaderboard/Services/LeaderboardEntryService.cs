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

        public LeaderboardEntryService()
        {
            _redis = ConnectionMultiplexer.Connect("localhost:5002");
            _db = _redis.GetDatabase(0);
        }
        /// <summary>
        /// Retrieves data from a 'simulated' database using a json file
        /// </summary>
        /// <returns>List<GetLeaderboardEntries>/returns>
        //public async Task<List<LeaderboardEntryModel>> GetLeaderboardEntries()
        //{
        //    StreamReader r = new StreamReader("Data/data.json");
        //    string json = r.ReadToEnd();

        //    var result = JsonSerializer.Deserialize<List<LeaderboardEntryModel>>(json);

        //    return result;
        //}

        public async Task<List<LeaderboardEntryModel>> GetLeaderboardEntries()
        {
            StreamReader r = new StreamReader("Data/data.json");
            string json = r.ReadToEnd();

            var result = JsonSerializer.Deserialize<List<LeaderboardEntryModel>>(json);

            foreach (var obj in result)
                await _db.SortedSetAddAsync("leaderboard", obj.username, obj.score);

            return result;
        }
    }
}
