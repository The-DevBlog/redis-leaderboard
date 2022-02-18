using Microsoft.Extensions.Caching.Distributed;
using RedisLeaderboard.Interfaces;
using RedisLeaderboard.Models;
using System.Text.Json;

namespace RedisLeaderboard.Services
{
    public class LeaderboardEntryService : ILeaderboardEntryService
    {
        public async Task<List<LeaderboardEntryModel>> GetLeaderboardEntries()
        {
            var result = new List<LeaderboardEntryModel>();

            return result;
        }
    }
}
