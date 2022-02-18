namespace RedisLeaderboard.Models
{
    public class LeaderboardEntryModel
    {
        public string Username { get; set; }
        public int Score { get; set; }
        public LeaderboardEntryModel(string username, int score)
        {
            Username = username;
            Score = score;
        }
    }
}
