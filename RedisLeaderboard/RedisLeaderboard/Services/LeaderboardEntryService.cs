using RedisLeaderboard.Interfaces;
using RedisLeaderboard.Models;
using System.Text.Json;

namespace RedisLeaderboard.Services
{
    public class LeaderboardEntryService : ILeaderboardEntryService
    {
        /// <summary>
        /// Retrieves data from a 'simulated' database using a json file
        /// </summary>
        /// <returns>List<GetLeaderboardEntries>/returns>
        public async Task<List<LeaderboardEntryModel>> GetLeaderboardEntries()
        {
            StreamReader r = new StreamReader("Data/data.json");
            string json = r.ReadToEnd();

            var result = JsonSerializer.Deserialize<List<LeaderboardEntryModel>>(json);

            return result;
        }
    }
}
