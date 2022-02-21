using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Xunit;

namespace RedisLeaderboardTests
{
    public class LeaderboardServiceTests
    {
        [Fact]
        public void Test1()
        {
            //_config = new IConfigurationBuilder();


            //var config = new ConfigurationBuilder()
            //                 .SetBasePath("C:/Users/Andrew/Desktop/code/projects/redis-leaderboard/RedisLeaderboard/RedisLeaderboard/")
            //                 .AddJsonFile("appsettings.json")
            //                 .Build();

            var redis = ConnectionMultiplexer.Connect("localhost:5002");
            var db = redis.GetDatabase(1);

            Assert.Equal("localhost:5002", tmp);
        }
    }
}