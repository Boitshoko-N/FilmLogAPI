using System.ComponentModel.DataAnnotations;

namespace FilmLogAPI.Models
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Genre { get; set; } = string.Empty;
        [Required]
        public string Year { get; set; } = string.Empty;
        [Required]
        public List<string> Actors { get; set; } = new List<string>();
        [Required]
        public string PosterUrl { get; set; } = string.Empty;
        public List<WatchListMovie> WatchlistMovies { get; set; } = new();
        public List<WatchedListMovie> WatchedListMovies { get; set; } = new();

    }
}
