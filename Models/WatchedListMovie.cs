namespace FilmLogAPI.Models
{
    public class WatchedListMovie
    {
        public int WatchedListId { get; set; }
        public WatchedList WatchedList { get; set; } = new WatchedList();

        public string MovieId { get; set; } = string.Empty;
        public Movie Movie { get; set; } = new Movie();

        public int TimesWatched { get; set; }
    }
}
