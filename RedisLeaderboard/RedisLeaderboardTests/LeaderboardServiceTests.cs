using Microsoft.Extensions.Configuration;
using RedisLeaderboard.Models;
using RedisLeaderboard.Services;
using StackExchange.Redis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace RedisLeaderboardTests
{
    public class IsolatedService
    {
        public ConnectionMultiplexer redis;
        public IDatabase db;
        public LeaderboardEntryService leaderboardService;
        public IsolatedService()
        {
            string path = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json")
                .Build();

            redis = ConnectionMultiplexer.Connect(config.GetConnectionString("Redis"));
            db = redis.GetDatabase(0);

            leaderboardService = new LeaderboardEntryService(config, db, redis);
        }
    }

    public class LeaderboardServiceTests
    {
        /// <summary>
        /// Tests GetEntriesForPage()
        /// </summary>
        /// <param name="expected">Expected result</param>
        /// <param name="pageNum">The 'page number' to get entries for</param>
        [Theory]
        [InlineData(10, 1)]
        [InlineData(10, 2)]
        [InlineData(6, 3)]
        public async Task CanGetLeaderboardEntriesForPage(int expected, int pageNum)
        {
            // arnge
            var service = new IsolatedService();
            await service.db.SortedSetRemoveRangeByRankAsync("leaderboard-tests", 0, -1);
            await service.leaderboardService.LoadDB("leaderboard-tests");

            // act
            var results = await service.leaderboardService.GetEntriesForPage(pageNum, 10, "leaderboard-tests");

            // assert
            Assert.Equal(expected, results.Count);
        }

        [Fact]
        public async Task CanAddEntry()
        {
            // arange
            var service = new IsolatedService();
            await service.db.SortedSetRemoveRangeByRankAsync("leaderboard-tests", 0, -1);
            await service.leaderboardService.LoadDB("leaderboard-tests");

            // act
            var newEntry = new LeaderboardEntryModel("TestUser", 678);
            await service.leaderboardService.AddEntry(newEntry, "leaderboard-tests");
            bool contains = await service.db.SortedSetRemoveAsync("leaderboard-tests", newEntry.username);

            // assert
            Assert.True(contains);
        }
    }
}