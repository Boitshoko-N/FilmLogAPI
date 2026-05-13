namespace FilmLogAPI.Models
{
    public class WatchListMovie
    {
        public int WatchlistId { get; set; }
        public Watchlist Watchlist { get; set; } = new Watchlist();

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = new Movie();

        public DateTime DateAdded { get; set; }
    }
}
