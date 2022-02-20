using RedisLeaderboard.Models;

namespace RedisLeaderboard.Interfaces
{
    public interface ILeaderboardEntryService
    {
        /// <summary>
        /// Returns the total number of leaderboard entries
        /// </summary>
        /// <returns>int</returns>
        int GetTotalCount();

        /// <summary>
        /// Returns a list of LeaderboardEntryModels for one page
        /// </summary>
        /// <param name="currentPg">The page number of the current page</param>
        /// <param name="perPage">Number of entries per page</param>
        /// <returns>List<LeaderboardEntryModel></returns>
        Task<List<LeaderboardEntryModel>> GetLeaderboardEntries(int currentPg, int perPage);

        /// <summary>
        /// Returns a list of LeaderboardEntryModels from the DB (json data)
        /// </summary>
        /// <returns>List<LeaderboardEntryModel></returns>
        Task LoadDB();

        /// <summary>
        /// Restores the leaderboard data with the default JSON data
        /// </summary>
        /// <returns>List<LeaderboardEntryModel></returns>
        Task<List<LeaderboardEntryModel>> RestoreDefaultData();

        /// <summary>
        /// Adds a leaderboard entry into the redis cache
        /// </summary>
        /// <param name="entry">New leaderboard entry</param>
        Task AddLeaderboardEntry(LeaderboardEntryModel entry);

        /// <summary>
        /// Deletes a leaderboard entry from the redis cache
        /// </summary>
        /// <param name="username">Leaderboard entry to delete</param>
        Task DeleteEntry(string username);
    }
}
