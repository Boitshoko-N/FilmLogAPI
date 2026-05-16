using FilmLogAPI.Data;
using FilmLogAPI.Models;
using Microsoft.EntityFrameworkCore;
using FilmLogAPI.DTOs;
using BCrypt.Net;

namespace FilmLogAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> LogInUser(LogInDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email.Trim().ToLower());

            if (user == null)
                return null;

            bool isValidPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isValidPassword)
                return null;

            return user;
        }

        public async Task<User?> AddUser(RegisterDto dto)
        {
            if (await UserExists(dto.Email))
                return null;

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}