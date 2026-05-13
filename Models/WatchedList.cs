using System.ComponentModel.DataAnnotations;

namespace FilmLogAPI.Models
{
    public class WatchedList
    {
        [Key]
        public int WatchedlistId { get; set; }
        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }
        public List<WatchedListMovie> WatchedListMovies { get; set; } = new();
    }
}
