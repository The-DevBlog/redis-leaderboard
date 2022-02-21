using Microsoft.Extensions.Configuration;
using RedisLeaderboard.Services;
using StackExchange.Redis;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace RedisLeaderboardTests
{
    public class LeaderboardServiceTests
    {
        private LeaderboardEntryService BuildService()
        {
            string path = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json")
                .Build();

            var redis = ConnectionMultiplexer.Connect(config.GetConnectionString("Redis"));
            var db = redis.GetDatabase(0);

            return new LeaderboardEntryService(db, redis);
        }

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
            var service = BuildService();
            await service.LoadDB("leaderboard-tests");
            var results = await service.GetEntriesForPage(pageNum, 10, "leaderboard-tests");

            Assert.Equal(expected, results.Count);
        }

        public async Task CanAddEntry()
        {
            var service = BuildService();
            await service.LoadDB("leaderboard-tests");

            var newEntry = new LeaderboardEntryModel();

            await service.AddEntry()

            Assert.Equal(expected, results.Count);
        }
    }
}