namespace RedisLeaderboard.Models
{
    public class LeaderboardEntry
    {
        public string Username { get; set; }
        public int Score { get; set; }
        public LeaderboardEntry(string username, int score)
        {
            Username = username;
            Score = score;
        }
    }
}
