using System.ComponentModel.DataAnnotations;

namespace FilmLogAPI.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public DateTime CreatedAt { get; set; }
        public Watchlist? Watchlist { get; set; }
        public WatchedList? WatchedList { get; set; }
    }
}
