using Microsoft.Extensions.Caching.Distributed;
using RedisLeaderboard.Models;
using System.Text.Json;

namespace RedisLeaderboard.RedisExtensions
{
    // class used from Tim Corey's YouTube tutorial: https://www.youtube.com/watch?v=UrQWii_kfIE
    public static class DistributeCache
    {
        public static async Task SetRecord<T>(this IDistributedCache cache,
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

        public static async Task<T> GetRecords<T>(this IDistributedCache cache, string recordId)
        {
            var jsonData = await cache.GetStringAsync(recordId);
            return jsonData == null ? default(T) : JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}
