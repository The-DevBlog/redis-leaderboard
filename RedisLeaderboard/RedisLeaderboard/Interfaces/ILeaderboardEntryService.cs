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
        /// <param name="db">Specified redis DB</param>
        /// <returns>List<LeaderboardEntryModel></returns>
        Task<List<LeaderboardEntryModel>> GetEntriesForPage(int currentPg, int perPage, string db = "leaderboard");

        /// <summary>
        /// Returns a list of LeaderboardEntryModels from the DB (json data)
        /// </summary>
        /// <param name="db">Specified redis DB</param>
        /// <returns>List<LeaderboardEntryModel></returns>
        Task LoadDB(string db = "leaderboard");

        /// <summary>
        /// Restores the leaderboard data with the default JSON data
        /// </summary>
        /// <param name="db">Specified redis DB</param>
        /// <returns>List<LeaderboardEntryModel></returns>
        Task<List<LeaderboardEntryModel>> RestoreDefaultData(string db = "leaderboard");

        /// <summary>
        /// Adds a leaderboard entry into the redis cache
        /// </summary>
        /// <param name="db">Specified redis DB</param>
        /// <param name="entry">New leaderboard entry</param>
        Task AddEntry(LeaderboardEntryModel entry, string db = "leaderboard");

        /// <summary>
        /// Deletes a leaderboard entry from the redis cache
        /// </summary>
        /// <param name="db">Specified redis DB</param>
        /// <param name="username">Leaderboard entry to delete</param>
        Task DeleteEntry(string username, string db = "leaderboard");
    }
}
