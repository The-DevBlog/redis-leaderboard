namespace RedisLeaderboard.Models
{
    public class LeaderboardEntryModel
    {
        public string username { get; set; }
        public int score { get; set; }

        public LeaderboardEntryModel(string username, int score)
        {
            this.username = username;
            this.score = score;
        }
    }
}
