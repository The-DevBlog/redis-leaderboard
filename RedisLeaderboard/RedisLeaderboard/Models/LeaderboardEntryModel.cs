using System.ComponentModel.DataAnnotations;

namespace RedisLeaderboard.Models
{
    public class LeaderboardEntryModel
    {
        [Required]
        public string username { get; set; }
        public int score { get; set; } = 0;

        public LeaderboardEntryModel(string username, int score)
        {
            this.username = username;
            this.score = score;
        }

        public LeaderboardEntryModel()
        {

        }
    }
}
