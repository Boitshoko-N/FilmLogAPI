using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FilmLogAPI.Repositories;
using FilmLogAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace FilmLogAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;

        public AuthController(IConfiguration config, IUserRepository userRepository)
        {
            _config = config;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = await _userRepository.AddUser(dto);

            if (user == null)
                return BadRequest("User already exists");

            return Ok(new
            {
                message = "User registered successfully",
                user = new
                {
                    user.UserId,
                    user.Email
                }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LogInDto dto)
        {
            var user = await _userRepository.LogInUser(dto);

            if (user == null)
                return Unauthorized("Invalid email or password");

            var token = GenerateToken(user.Email, user.UserId);

            return Ok(new
            {
                token,
                user = new
                {
                    user.UserId,
                    user.Email
                }
            });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var user = await _userRepository.GetUserById(userId);

            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.UserId,
                user.Email,
                user.CreatedAt
            });
        }

        [HttpGet("exists")]
        public async Task<IActionResult> UserExists([FromQuery] string email)
        {
            var exists = await _userRepository.UserExists(email);
            return Ok(new { exists });
        }

        private string GenerateToken(string email, int userId)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var jwtKey = _config["Jwt:Key"]
              ?? throw new InvalidOperationException("JWT Key is missing in configuration");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}