using System.ComponentModel.DataAnnotations;

namespace RedisLeaderboard.Models
{
    public class LeaderboardEntryModel
    {
        [Required]
        public string username { get; set; }
        [Required]
        public int score { get; set; }

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
