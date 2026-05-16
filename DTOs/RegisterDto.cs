namespace FilmLogAPI.DTOs
{
    public class RegisterDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
