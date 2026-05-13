using System.ComponentModel.DataAnnotations;

namespace FilmLogAPI.Models
{
    public class Watchlist
    {
        [Key]
        public int WatchlistId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public User? User { get; set; }

        public List<WatchListMovie> WatchlistMovies { get; set; } = new();

    }
}
