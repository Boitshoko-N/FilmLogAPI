namespace FilmLogAPI.Models
{
    public class WatchedListMovie
    {
        public int WatchedListId { get; set; }
        public WatchedList WatchedList { get; set; } = new WatchedList();

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = new Movie();

        public int TimesWatched { get; set; }
    }
}
