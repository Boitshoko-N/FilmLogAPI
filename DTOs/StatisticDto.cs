namespace FilmLogAPI.DTOs
{
    public class GenreStatsDto
    {
        public string Genre { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class YearStatsDto
    {
        public string Year { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class MovieStatsDto
    {
        public string Title { get; set; } = string.Empty;
        public int TimesWatched { get; set; }
    }

    public class StatisticsDashboardDto
    {
        public List<GenreStatsDto> TopGenres { get; set; } = new List<GenreStatsDto>();
        public List<YearStatsDto> MoviesByYear { get; set; } = new List<YearStatsDto>();
        public List<MovieStatsDto> TopMovies { get; set; } = new List<MovieStatsDto>();
    }
}