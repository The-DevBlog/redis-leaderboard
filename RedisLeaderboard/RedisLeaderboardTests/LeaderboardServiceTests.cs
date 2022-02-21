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
            // arrange
            var service = new IsolatedService();
            await service.db.SortedSetRemoveRangeByRankAsync("leaderboard-tests", 0, -1);
            await service.leaderboardService.LoadDB("leaderboard-tests");

            // act
            var results = await service.leaderboardService.GetEntriesForPage(pageNum, 10, "leaderboard-tests");

            // assert
            Assert.Equal(expected, results.Count);
        }

        /// <summary>
        /// Tests AddEntry()
        /// </summary>
        [Fact]
        public async Task CanAddEntry()
        {
            // arrange
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

        /// <summary>
        /// Tests DeleteEntry()
        /// </summary>
        [Fact]
        public async Task CanDeleteEntry()
        {
            // arrange 
            var service = new IsolatedService();
            await service.db.SortedSetRemoveRangeByRankAsync("leaderboard-tests", 0, -1);
            await service.leaderboardService.LoadDB("leaderboard-tests");

            // act
            await service.leaderboardService.DeleteEntry("Frodo", "leaderboard-tests");
            bool contains = await service.db.SortedSetRemoveAsync("leaderboard-tests", "Frodo");

            // assert 
            Assert.False(contains);
        }

        [Fact]
        public async Task CanRestoreDbWithDefaultData()
        {
            // arrange
            var service = new IsolatedService();
            await service.db.SortedSetRemoveRangeByRankAsync("leaderboard-tests", 0, -1);

            // act
            await service.db.SortedSetAddAsync("leaderboard-tests", "Newuser", 100);
            await service.db.SortedSetAddAsync("leaderboard-tests", "Newuser2", 200);
            await service.leaderboardService.RestoreDefaultData("leaderboard-tests");
            var results = await service.db.SortedSetRangeByRankAsync("leaderboard-tests", 0, -1);

            // assert
            Assert.Equal(26, results.Length);

        }

        /// <summary>
        /// Tests LoadDB()
        /// </summary>
        [Fact]
        public async Task CanInitializeDbWithDefaultData()
        {
            // arrange 
            var service = new IsolatedService();
            await service.db.SortedSetRemoveRangeByRankAsync("leaderboard-tests", 0, -1);

            // act
            await service.leaderboardService.LoadDB("leaderboard-tests");
            var results = await service.db.SortedSetRangeByRankAsync("leaderboard-tests", 0, -1);

            // assert
            Assert.Equal(26, results.Length);
        }
    }
}