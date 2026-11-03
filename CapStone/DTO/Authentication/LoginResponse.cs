namespace CapStone.DTO.Authentication
{
    public class LoginResponseDTO
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        // Added to match PDF SRS: "Token type"
        public string TokenType { get; set; } = "Bearer";

        public DateTime Expiration { get; set; }
    }
}