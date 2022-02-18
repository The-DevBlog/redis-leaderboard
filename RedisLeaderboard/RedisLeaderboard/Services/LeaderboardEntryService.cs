using RedisLeaderboard.Models;
using StackExchange.Redis;

namespace RedisLeaderboard.Services
{
    public class LeaderboardEntryService
    {
        public async Task<List<LeaderboardEntryModel>> GetEntries()
        {
            var result = new List<LeaderboardEntryModel>();

            return result;
        }
    }
}
