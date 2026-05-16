namespace FilmLogAPI.DTOs
{
    public class LogInDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
