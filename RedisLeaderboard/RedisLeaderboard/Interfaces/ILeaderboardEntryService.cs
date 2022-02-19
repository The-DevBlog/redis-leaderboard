using RedisLeaderboard.Models;

namespace RedisLeaderboard.Interfaces
{
    public interface ILeaderboardEntryService
    {
        Task<List<LeaderboardEntryModel>> GetLeaderboardEntries(List<LeaderboardEntryModel> currentEntries);
        Task AddLeaderboardEntry(LeaderboardEntryModel entry);
        Task DeleteEntry(string username);
    }
}
