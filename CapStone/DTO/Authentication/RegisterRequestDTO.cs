using System.ComponentModel.DataAnnotations;

namespace CapStone.DTO.Authentication
{
    public class RegisterRequestDTO
    {
        /// <summary>
        /// Full Name of the User
        /// </summary>
        /// <example>John Doe</example>
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Email Address of the User
        /// </summary>
        /// <example>johndoe@example.com</example>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password of the User
        /// </summary>
        /// <example>Password123</example>
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 10-digit mobile number starting with 6, 7, 8, or 9
        /// </summary>
        /// <example>9876543210</example>
        [Required]
        [RegularExpression(@"^[6-9]\d{9}$")]
        public string MobileNumber { get; set; } = string.Empty;
    }
}