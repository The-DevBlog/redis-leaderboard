using RedisLeaderboard.Models;

namespace RedisLeaderboard.Interfaces
{
    public interface ILeaderboardEntryService
    {
        /// <summary>
        /// Retrieves data from a 'simulated' database using a json file
        /// </summary>
        /// <returns>List<GetLeaderboardEntries>/returns>
        Task<List<LeaderboardEntryModel>> GetLeaderboardEntries();
    }
}
