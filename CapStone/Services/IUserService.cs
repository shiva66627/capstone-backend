using CapStone.DTO.Common;
using CapStone.DTO.User;

namespace CapStone.Services
{
    public interface IUserService
    {
        Task<ApiResponseDTO<IEnumerable<UserResponseDTO>>> GetAllUsersAsync();

        Task<ApiResponseDTO<PaginatedResponseDTO<UserResponseDTO>>> GetPagedUsersAsync(UserFilterDTO filter);

        Task<ApiResponseDTO<UserResponseDTO>> GetUserByIdAsync(int id);

        Task<ApiResponseDTO<string>> UpdateUserStatusAsync(int id, UserStatusUpdateDTO request);

        Task<ApiResponseDTO<string>> DeactivateUserAsync(int id);

        Task<ApiResponseDTO<string>> DeleteUserAsync(int id);

        Task<ApiResponseDTO<string>> PromoteToStaffAsync(PromoteToStaffRequestDTO request);

        Task<ApiResponseDTO<string>> DemoteToCustomerAsync(PromoteToStaffRequestDTO request);
    }
}