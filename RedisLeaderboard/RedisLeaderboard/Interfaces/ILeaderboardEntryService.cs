using RedisLeaderboard.Models;

namespace RedisLeaderboard.Interfaces
{
    public interface ILeaderboardEntryService
    {
        Task<List<LeaderboardEntryModel>> GetLeaderboardEntries(List<LeaderboardEntryModel> currentEntries, int currentPg);
        Task AddLeaderboardEntry(LeaderboardEntryModel entry);
        Task DeleteEntry(string username);
        int GetTotalCount();
    }
}
