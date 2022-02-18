using Microsoft.Extensions.Caching.Distributed;
using RedisLeaderboard.Models;
using System.Text.Json;

namespace RedisLeaderboard.Services
{
    public class LeaderboardEntryService
    {
        public async Task<List<LeaderboardEntryModel>> GetLeaderboardEntries()
        {
            var result = new List<LeaderboardEntryModel>();

            return result;
        }
    }

    // class used from Tim Corey's YouTube tutorial: https://www.youtube.com/watch?v=UrQWii_kfIE
    public static class DistributeCacheExtensions
    {
        private static async Task Set<T>(this IDistributedCache cache,
            string recordId,
            T data,
            TimeSpan? absoluteExpireTime = null,
            TimeSpan? unusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions();

            options.AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60);
            options.SlidingExpiration = unusedExpireTime;

            var jsonData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(recordId, jsonData, options);
        }

        private static async Task<T> Get<T>(this IDistributedCache cache, string recordId)
        {
            var jsonData = await cache.GetStringAsync(recordId);
            var deserializedData = JsonSerializer.Deserialize<T>(jsonData);

            return jsonData == null ? default(T) : deserializedData;
        }
    }
}
