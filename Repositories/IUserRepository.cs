using FilmLogAPI.DTOs;
using FilmLogAPI.Models;

namespace FilmLogAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserById(int id);
        Task<User?> LogInUser(LogInDto dto);
        Task<User?>AddUser(RegisterDto dto);
        Task<bool>UserExists(string  email);
    }
}
