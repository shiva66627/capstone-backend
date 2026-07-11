using BCrypt.Net;
using CapStone.Configuration;
using CapStone.DTO.Authentication;
using CapStone.DTO.Common;
using CapStone.Models;
using CapStone.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtClaim = System.Security.Claims.Claim;
namespace CapStone.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IUserRepository userRepository,
          
            IOptions<JwtSettings> jwtOptions)
        {
            _userRepository = userRepository;
            
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<ApiResponseDTO<string>> RegisterAsync(RegisterRequestDTO request)
        {
            try
            {
                // Check if email already exists
                if (await _userRepository.EmailExistsAsync(request.Email))
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Email already exists."
                    };
                }

                // Check if mobile number already exists
                if (await _userRepository.MobileExistsAsync(request.MobileNumber))
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Mobile number already exists."
                    };
                }

                // Map DTO to User entity
                // Create User entity manually
                var user = new User
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    MobileNumber = request.MobileNumber,
                    Role = Role.Customer,
                    ActiveStatus = ActiveStatusEnum.Active,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

              

                // Save user
                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Registration successful.",
                    Data = user.Email
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        private LoginResponseDTO GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes);

            var claims = new List<JwtClaim>
{
    new JwtClaim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
    new JwtClaim(ClaimTypes.Name, user.UserName),
    new JwtClaim(ClaimTypes.Email, user.Email),
    new JwtClaim(ClaimTypes.Role, user.Role.ToString())
};

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),

                Expires = expiration,

                Issuer = _jwtSettings.Issuer,

                Audience = _jwtSettings.Audience,

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginResponseDTO
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = tokenHandler.WriteToken(token),
                Expiration = expiration
            };
        }
        public async Task<ApiResponseDTO<LoginResponseDTO>> LoginAsync(LoginRequestDTO request)
        {
            try
            {
                // Check whether user exists
                var user = await _userRepository.GetByEmailAsync(request.Email);

                if (user == null)
                {
                    return new ApiResponseDTO<LoginResponseDTO>
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                // Check user status
                if (user.ActiveStatus != ActiveStatusEnum.Active)
                {
                    return new ApiResponseDTO<LoginResponseDTO>
                    {
                        Success = false,
                        Message = "Your account is inactive."
                    };
                }

                // Verify password
                bool passwordMatched = BCrypt.Net.BCrypt.Verify(
                    request.Password,
                    user.PasswordHash);

                if (!passwordMatched)
                {
                    return new ApiResponseDTO<LoginResponseDTO>
                    {
                        Success = false,
                        Message = "Invalid email or password."
                    };
                }

                // Generate JWT
                var loginResponse = GenerateJwtToken(user);

                return new ApiResponseDTO<LoginResponseDTO>
                {
                    Success = true,
                    Message = "Login successful.",
                    Data = loginResponse
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<LoginResponseDTO>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}