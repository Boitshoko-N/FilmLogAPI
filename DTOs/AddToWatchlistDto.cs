namespace FilmLogAPI.DTOs
{
    public class AddToWatchlistDto
    {
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Actors { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
    }
}
