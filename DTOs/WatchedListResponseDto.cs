namespace FilmLogAPI.DTOs
{
    public class WatchedListResponseDto
    {
        public int WatchedListId { get; set; }
        public List<WatchedMovieDto> Movies { get; set; } = new();
    }

    public class WatchedMovieDto
    {
        public string MovieId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public int TimesWatched { get; set; }
    }
}