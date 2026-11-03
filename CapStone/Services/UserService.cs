using CapStone.DTO.Common;
using CapStone.DTO.User;
using CapStone.Exceptions;
using CapStone.Models;
using CapStone.Repositories;

namespace CapStone.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponseDTO<IEnumerable<UserResponseDTO>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();

                var result = users.Select(u => new UserResponseDTO
                {
                    UserId = u.UserId,
                    FullName = u.UserName,
                    Email = u.Email,
                    MobileNumber = u.MobileNumber,
                    Role = u.Role,
                    ActiveStatus = u.ActiveStatus,
                    CreatedDate = u.CreatedDate
                });

                return new ApiResponseDTO<IEnumerable<UserResponseDTO>>
                {
                    Success = true,
                    Message = "Users retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<IEnumerable<UserResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<PaginatedResponseDTO<UserResponseDTO>>> GetPagedUsersAsync(UserFilterDTO filter)
        {
            try
            {
                var (users, totalRecords) = await _userRepository.GetFilteredUsersAsync(filter);

                var userDtos = users.Select(u => new UserResponseDTO
                {
                    UserId = u.UserId,
                    FullName = u.UserName,
                    Email = u.Email,
                    MobileNumber = u.MobileNumber,
                    Role = u.Role,
                    ActiveStatus = u.ActiveStatus,
                    CreatedDate = u.CreatedDate
                });

                var paginatedData = new PaginatedResponseDTO<UserResponseDTO>(
                    userDtos, totalRecords, filter.PageNumber, filter.PageSize, filter.SortField, filter.SortDirection);

                return new ApiResponseDTO<PaginatedResponseDTO<UserResponseDTO>>
                {
                    Success = true,
                    Message = "Users retrieved successfully.",
                    Data = paginatedData
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<PaginatedResponseDTO<UserResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<UserResponseDTO>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);

                if (user == null)
                {
                    throw new ResourceNotFoundException("User not found.");
                }

                var dto = new UserResponseDTO
                {
                    UserId = user.UserId,
                    FullName = user.UserName,
                    Email = user.Email,
                    MobileNumber = user.MobileNumber,
                    Role = user.Role,
                    ActiveStatus = user.ActiveStatus,
                    CreatedDate = user.CreatedDate
                };

                return new ApiResponseDTO<UserResponseDTO>
                {
                    Success = true,
                    Message = "User retrieved successfully.",
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<UserResponseDTO>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<string>> UpdateUserStatusAsync(int id, UserStatusUpdateDTO request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);

                if (user == null)
                {
                    throw new ResourceNotFoundException("User not found.");
                }

                user.ActiveStatus = request.ActiveStatus;
                user.UpdatedDate = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "User status updated successfully."
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<string>> DeactivateUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);

                if (user == null)
                {
                    throw new ResourceNotFoundException("User not found.");
                }

                user.ActiveStatus = ActiveStatusEnum.InActive;
                user.UpdatedDate = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "User deactivated successfully."
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<string>> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);

                if (user == null)
                {
                    throw new ResourceNotFoundException("User not found.");
                }

                await _userRepository.DeleteAsync(id);
                await _userRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "User deleted successfully."
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<string>> PromoteToStaffAsync(PromoteToStaffRequestDTO request)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(request.Email);

                if (user == null)
                {
                    throw new ResourceNotFoundException("User not found.");
                }

                user.Role = Role.Staff;
                user.UpdatedDate = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "User successfully promoted to Staff role."
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<string>> DemoteToCustomerAsync(PromoteToStaffRequestDTO request)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(request.Email);

                if (user == null)
                {
                    throw new ResourceNotFoundException("User not found.");
                }

                if (user.Role == Role.Customer)
                {
                    throw new BusinessRuleViolationException("User is already a Customer.");
                }

                user.Role = Role.Customer;
                user.UpdatedDate = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "User successfully demoted to Customer role."
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}