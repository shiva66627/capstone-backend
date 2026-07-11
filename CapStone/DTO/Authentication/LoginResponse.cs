namespace CapStone.DTO.Authentication
{
    public class LoginResponseDTO
    {
        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        public DateTime Expiration { get; set; }
    }
}