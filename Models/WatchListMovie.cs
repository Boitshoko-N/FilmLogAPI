namespace FilmLogAPI.Models
{
    public class WatchListMovie
    {
        public int WatchlistId { get; set; }
        public Watchlist Watchlist { get; set; } = new Watchlist();

        public string MovieId { get; set; } = string.Empty;
        public Movie Movie { get; set; } = new Movie();

        public DateTime DateAdded { get; set; }
    }
}
