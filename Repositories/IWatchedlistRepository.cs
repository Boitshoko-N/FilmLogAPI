using FilmLogAPI.DTOs;
using FilmLogAPI.Models;

namespace FilmLogAPI.Repositories
{
    public interface IWatchedlistRepository
    {
        Task<IEnumerable<WatchedMovieDto>> GetUserWatchedlist(int userId);
        Task<WatchedList> AddToWatchedlist(int userId, MovieDetailsDto movieDto);
        Task<bool> RemoveFromWatchedlist(int userId, string imdbId);
        Task<bool> MovieExists(int userId, string imdbId);
        Task<bool> ResetTimesWatched(int userId, string imdbId);
        Task<StatisticsDashboardDto>GetStatistics(int userId);
    }
}