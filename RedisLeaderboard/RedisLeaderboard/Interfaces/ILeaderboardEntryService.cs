using RedisLeaderboard.Models;

namespace RedisLeaderboard.Interfaces
{
    public interface ILeaderboardEntryService
    {
        Task<List<LeaderboardEntryModel>> GetLeaderboardEntries(List<LeaderboardEntryModel> currentEntries);
        Task<List<LeaderboardEntryModel>> AddLeaderboardEntry(LeaderboardEntryModel entry);
    }
}
