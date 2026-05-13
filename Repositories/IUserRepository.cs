using FilmLogAPI.Models;

namespace FilmLogAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserById(int id);
        Task<User?> GetUserByEmail(string email);
        Task<User>AddUser(User user);
        Task<bool>UserExists(string  email);
    }
}
