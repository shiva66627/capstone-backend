using BCrypt.Net;
using CapStone.Configuration;
using CapStone.DTO.Authentication;
using CapStone.DTO.Common;
using CapStone.Exceptions;
using CapStone.Models;
using CapStone.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using JwtClaim = System.Security.Claims.Claim;
namespace CapStone.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailService _emailService;

        public AuthService(
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            IOptions<JwtSettings> jwtOptions,
            ILogger<AuthService> logger,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _jwtSettings = jwtOptions.Value;
            _logger = logger;
            _emailService = emailService;
        }


        public async Task<ApiResponseDTO<string>> RegisterAsync(
            RegisterRequestDTO request)
        {
            try
            {
                if (await _userRepository.EmailExistsAsync(request.Email))
                {
                    throw new DuplicateEmailException(
                        "Email already exists.");
                }

                if (await _userRepository.MobileExistsAsync(
                    request.MobileNumber))
                {
                    throw new DuplicateEmailException(
                        "Mobile number already exists.");
                }

                // Generate a cryptographically secure 6-digit OTP.
                var otp = RandomNumberGenerator
                    .GetInt32(100000, 1000000)
                    .ToString();

                // Create the user and store only the OTP hash.
                var user = new User
                {
                    UserName = request.FullName,
                    Email = request.Email,
                    PasswordHash =
                        BCrypt.Net.BCrypt.HashPassword(request.Password),
                    MobileNumber = request.MobileNumber,
                    Role = Role.Customer,
                    ActiveStatus = ActiveStatusEnum.Active,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,

                    IsEmailVerified = false,
                    EmailOtpHash = BCrypt.Net.BCrypt.HashPassword(otp),
                    EmailOtpExpiry = DateTime.UtcNow.AddMinutes(5),
                    EmailOtpAttempts = 0
                };

                // Save user first.
                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                if (user.Role == Role.Customer)
                {
                    var customer = new Customer
                    {
                        UserId = user.UserId,
                        Gender = Gender.Male,
                        DateOfBirth = new DateOnly(2000, 1, 1),
                        City = "Not Updated",
                        Address = "Not Updated",
                        State = "Not Updated",
                        Pincode = "000000",
                        NomineeName = "Not Updated",
                        NomineeRelation = "Not Updated",
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow
                    };
                    await _customerRepository.AddAsync(customer);
                    await _customerRepository.SaveChangesAsync();
                }

                // Send OTP to the registered email.
                await _emailService.SendOtpEmailAsync(user.Email, otp);

                _logger.LogInformation(
                    "Registration completed and OTP email sent for user {UserId}",
                    user.UserId);

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message =
                        "Registration successful. Please verify the OTP sent to your email.",
                    Data = user.Email
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions")
                    throw;

                _logger.LogError(
                    ex,
                    "Registration or OTP email delivery failed.");

                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = "Registration could not be completed. Please check the server logs."
                };
            }
        }

        private LoginResponseDTO 
            GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes);

            var claims = new List<JwtClaim>
{
    new JwtClaim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
    new JwtClaim(ClaimTypes.Name, user.UserName),
    new JwtClaim(ClaimTypes.Email, user.Email),
    new JwtClaim(ClaimTypes.Role, user.Role.ToString()),
    new JwtClaim("role", user.Role.ToString())
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
                FullName = user.UserName,
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
                    _logger.LogWarning("LOG-003: Login failure for email: {Email}. Reason: User not found.", request.Email);
                    throw new InvalidCredentialsException("Invalid email or password.");
                }

                // Check user status
                if (user.ActiveStatus != ActiveStatusEnum.Active)
                {
                    _logger.LogWarning("LOG-014: Security failure. Inactive user attempted login: {Email}", request.Email);
                    throw new InactiveUserException("Your account is inactive.");
                }
                if (!user.IsEmailVerified)
                {
                    return new ApiResponseDTO<LoginResponseDTO>
                    {
                        Success = false,
                        Message = "Please verify your email before logging in."
                    };
                }

                // Verify password
                bool passwordMatched = BCrypt.Net.BCrypt.Verify(
                    request.Password,
                    user.PasswordHash);

                if (!passwordMatched)
                {
                    _logger.LogWarning("LOG-003: Login failure for email: {Email}. Reason: Password mismatch.", request.Email);
                    throw new InvalidCredentialsException("Invalid email or password.");
                }

                // Generate JWT
                var loginResponse = GenerateJwtToken(user);

                _logger.LogInformation("LOG-002: Login success for user: {Email}", request.Email);

                return new ApiResponseDTO<LoginResponseDTO>
                {
                    Success = true,
                    Message = "Login successful.",
                    Data = loginResponse
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                _logger.LogError(ex, "LOG-016: Unexpected system error during login for: {Email}", request.Email);
                return new ApiResponseDTO<LoginResponseDTO>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        //public async Task<ApiResponseDTO<LoginResponseDTO>>
        //AdminLoginAsync(LoginRequestDTO request)
        //{
        //    try
        //    {
        //        var user = await _userRepository.GetByEmailAsync(request.Email);

        //        if (user == null)
        //        {
        //            _logger.LogWarning("LOG-003: Login failure during Admin login for email: {Email}. Reason: User not found.", request.Email);
        //            throw new InvalidCredentialsException("Invalid credentials");
        //        }

        //        if (user.Role != Role.Admin)
        //        {
        //            _logger.LogWarning("LOG-014: Security failure. Non-admin user attempted admin login: {Email}", request.Email);
        //            throw new ForbiddenAccessException("Only admin can login here.");
        //        }

        //        bool verified = BCrypt.Net.BCrypt.Verify(
        //            request.Password,
        //            user.PasswordHash);

        //        if (!verified)
        //        {
        //            _logger.LogWarning("LOG-003: Login failure during Admin login for email: {Email}. Reason: Password mismatch.", request.Email);
        //            throw new InvalidCredentialsException("Invalid credentials");
        //        }

        //        _logger.LogInformation("LOG-002: Login success during Admin login for email: {Email}", request.Email);

        //        return new ApiResponseDTO<LoginResponseDTO>
        //        {
        //            Success = true,
        //            Message = "Admin login successful",
        //            Data = GenerateJwtToken(user)
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
        //        _logger.LogError(ex, "LOG-016: Unexpected system error during Admin login for: {Email}", request.Email);
        //        return new ApiResponseDTO<LoginResponseDTO>
        //        {
        //            Success = false,
        //            Message = ex.Message
        //        };
        //    }
        //}
        public async Task<ApiResponseDTO<string>> VerifyOtpAsync(
    VerifyOtpRequestDTO request)
        {
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = "Invalid email or OTP."
                };
            }

            // Check if email is already verified
            if (user.IsEmailVerified)
            {
                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Email is already verified."
                };
            }

            // Check if OTP exists
            if (string.IsNullOrEmpty(user.EmailOtpHash) ||
                user.EmailOtpExpiry == null)
            {
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = "No valid OTP found. Please register again or request a new OTP."
                };
            }

            // Check OTP expiry
            if (user.EmailOtpExpiry.Value <= DateTime.UtcNow)
            {
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = "OTP has expired. Please request a new OTP."
                };
            }

            // Check maximum attempts
            if (user.EmailOtpAttempts >= 5)
            {
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = "Maximum OTP attempts exceeded. Please request a new OTP."
                };
            }

            // Increment attempts before checking the OTP
            user.EmailOtpAttempts++;

            if (!BCrypt.Net.BCrypt.Verify(
                request.Otp,
                user.EmailOtpHash))
            {
                await _userRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = "Invalid email or OTP."
                };
            }

            // OTP verified successfully
            user.IsEmailVerified = true;
            user.EmailOtpHash = null;
            user.EmailOtpExpiry = null;
            user.EmailOtpAttempts = 0;
            user.UpdatedDate = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Email verification successful for user: {UserId}",
                user.UserId);

            return new ApiResponseDTO<string>
            {
                Success = true,
                Message = "Email verified successfully."
            };
        }
        public async Task<ApiResponseDTO<string>> ResendOtpAsync(
    ResendOtpRequestDTO request)
        {
            try
            {
                // 1. Find the user by email
                var user = await _userRepository.GetByEmailAsync(request.Email);

                if (user == null)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Unable to resend OTP. Please check your email."
                    };
                }

                // 2. Check whether the email is already verified
                if (user.IsEmailVerified)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Email is already verified."
                    };
                }

                // 3. Prevent repeated requests within 60 seconds
                // Requires the EmailOtpLastSentAt property added below.
                if (user.EmailOtpLastSentAt.HasValue &&
                    DateTime.UtcNow <
                    user.EmailOtpLastSentAt.Value.AddSeconds(60))
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Please wait 60 seconds before requesting another OTP."
                    };
                }

                // 4. Generate a new six-digit OTP
                var otp = RandomNumberGenerator
                    .GetInt32(100000, 1000000)
                    .ToString();

                // 5. Store the hashed OTP and its expiry
                user.EmailOtpHash = BCrypt.Net.BCrypt.HashPassword(otp);
                user.EmailOtpExpiry = DateTime.UtcNow.AddMinutes(5);
                user.EmailOtpAttempts = 0;
                user.EmailOtpLastSentAt = DateTime.UtcNow;
                user.UpdatedDate = DateTime.UtcNow;

                // 6. Save the new OTP details
                await _userRepository.SaveChangesAsync();

                // 7. Send the OTP to the user's email
                await _emailService.SendOtpEmailAsync(user.Email, otp);

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "A new OTP has been sent to your email.",
                    Data = user.Email
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resend OTP failed.");

                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = "Unable to resend OTP. Please try again later."
                };
            }
        }
    }
}