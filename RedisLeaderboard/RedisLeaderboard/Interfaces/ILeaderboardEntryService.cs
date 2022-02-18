using RedisLeaderboard.Models;

namespace RedisLeaderboard.Interfaces
{
    public interface ILeaderboardEntryService
    {
        Task<LeaderboardEntryModel> GetEntries();
    }
}
