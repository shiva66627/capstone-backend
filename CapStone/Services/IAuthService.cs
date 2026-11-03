using CapStone.DTO.Authentication;
using CapStone.DTO.Common;

namespace CapStone.Services
{
    public interface IAuthService
    {
        Task<ApiResponseDTO<string>> RegisterAsync(RegisterRequestDTO request);

        Task<ApiResponseDTO<LoginResponseDTO>> LoginAsync(LoginRequestDTO request);
        Task<ApiResponseDTO<string>> VerifyOtpAsync(
          VerifyOtpRequestDTO request);
        Task<ApiResponseDTO<string>> ResendOtpAsync(
    ResendOtpRequestDTO request);
        //Task<ApiResponseDTO<LoginResponseDTO>>AdminLoginAsync(LoginRequestDTO request);
    }
}