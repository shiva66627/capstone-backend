using CapStone.DTO.Common;
using CapStone.DTO.Customer;
using CapStone.Exceptions;
using CapStone.Models;
using CapStone.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace CapStone.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(
            ICustomerRepository customerRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ApiResponseDTO<IEnumerable<CustomerResponseDTO>>> GetAllCustomersAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();

                var result = customers.Select(c => new CustomerResponseDTO
                {
                    CustomerId = c.CustomerId,
                    FullName = c.User != null ? c.User.UserName : string.Empty,
                    Email = c.User != null ? c.User.Email : string.Empty,
                    MobileNumber = c.User != null ? c.User.MobileNumber : string.Empty,
                    DateOfBirth = c.DateOfBirth,
                    Gender = c.Gender,
                    City = c.City,
                    Address = c.Address,
                    State = c.State,
                    Pincode = c.Pincode,
                    NomineeName = c.NomineeName,
                    NomineeRelation = c.NomineeRelation,
                    UserId = c.UserId,
                    CreatedDate = c.CreatedDate
                });

                return new ApiResponseDTO<IEnumerable<CustomerResponseDTO>>
                {
                    Success = true,
                    Message = "Customers retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<IEnumerable<CustomerResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<PaginatedResponseDTO<CustomerResponseDTO>>> GetPagedCustomersAsync(CustomerFilterDTO filter)
        {
            try
            {
                var (customers, totalRecords) = await _customerRepository.GetFilteredCustomersAsync(filter);

                var result = customers.Select(c => new CustomerResponseDTO
                {
                    CustomerId = c.CustomerId,
                    FullName = c.User != null ? c.User.UserName : string.Empty,
                    Email = c.User != null ? c.User.Email : string.Empty,
                    MobileNumber = c.User != null ? c.User.MobileNumber : string.Empty,
                    DateOfBirth = c.DateOfBirth,
                    Gender = c.Gender,
                    City = c.City,
                    Address = c.Address,
                    State = c.State,
                    Pincode = c.Pincode,
                    NomineeName = c.NomineeName,
                    NomineeRelation = c.NomineeRelation,
                    UserId = c.UserId,
                    CreatedDate = c.CreatedDate
                });

                var paginatedData = new PaginatedResponseDTO<CustomerResponseDTO>(
                    result, totalRecords, filter.PageNumber, filter.PageSize, filter.SortField, filter.SortDirection);

                return new ApiResponseDTO<PaginatedResponseDTO<CustomerResponseDTO>>
                {
                    Success = true,
                    Message = "Customers retrieved successfully.",
                    Data = paginatedData
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<PaginatedResponseDTO<CustomerResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<CustomerResponseDTO>> GetCustomerByIdAsync(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);

                if (customer == null)
                {
                    throw new ResourceNotFoundException("Customer not found.");
                }

                var currentUserRole = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value);
                if (currentUserRole == "Customer")
                {
                    var currentUserIdClaim = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value);
                    int.TryParse(currentUserIdClaim, out int currentUserId);
                    if (customer.UserId != currentUserId)
                    {
                        _logger.LogWarning("LOG-014: Security failure. Customer {UserId} attempted to view unauthorized customer profile: {CustomerId}", currentUserId, id);
                        throw new ForbiddenAccessException("You are not authorized to view this customer profile.");
                    }
                }

                var result = new CustomerResponseDTO
                {
                    CustomerId = customer.CustomerId,
                    FullName = customer.User != null ? customer.User.UserName : string.Empty,
                    Email = customer.User != null ? customer.User.Email : string.Empty,
                    MobileNumber = customer.User != null ? customer.User.MobileNumber : string.Empty,
                    DateOfBirth = customer.DateOfBirth,
                    Gender = customer.Gender,
                    City = customer.City,
                    Address = customer.Address,
                    State = customer.State,
                    Pincode = customer.Pincode,
                    NomineeName = customer.NomineeName,
                    NomineeRelation = customer.NomineeRelation,
                    UserId = customer.UserId,
                    CreatedDate = customer.CreatedDate
                };

                return new ApiResponseDTO<CustomerResponseDTO>
                {
                    Success = true,
                    Message = "Customer retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                _logger.LogError(ex, "LOG-016: Unexpected system error during GetCustomerById.");
                return new ApiResponseDTO<CustomerResponseDTO>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<CustomerResponseDTO>> GetCustomerByUserIdAsync(int userId)
        {
            try
            {
                var currentUserRole = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value);
                if (currentUserRole == "Customer")
                {
                    var currentUserIdClaim = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value);
                    int.TryParse(currentUserIdClaim, out int currentUserId);
                    if (userId != currentUserId)
                    {
                        _logger.LogWarning("LOG-014: Security failure. Customer {UserId} attempted to view unauthorized customer profile by UserId: {ReqUserId}", currentUserId, userId);
                        throw new ForbiddenAccessException("You are not authorized to view this customer profile.");
                    }
                }

                var customers = await _customerRepository.FindAsync(c => c.UserId == userId);
                var customer = customers.FirstOrDefault();

                if (customer == null)
                {
                    var user = await _userRepository.GetByIdAsync(userId);
                    if (user != null && user.Role == Role.Customer)
                    {
                        customer = new Customer
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
                        customer.User = user;
                    }
                    else
                    {
                        throw new ResourceNotFoundException("Customer not found for this user.");
                    }
                }

                // Eager loading user if missing
                if (customer.User == null)
                {
                    var loaded = await _customerRepository.GetByIdAsync(customer.CustomerId);
                    if (loaded != null) customer = loaded;
                }

                var result = new CustomerResponseDTO
                {
                    CustomerId = customer.CustomerId,
                    FullName = customer.User != null ? customer.User.UserName : string.Empty,
                    Email = customer.User != null ? customer.User.Email : string.Empty,
                    MobileNumber = customer.User != null ? customer.User.MobileNumber : string.Empty,
                    DateOfBirth = customer.DateOfBirth,
                    Gender = customer.Gender,
                    City = customer.City,
                    Address = customer.Address,
                    State = customer.State,
                    Pincode = customer.Pincode,
                    NomineeName = customer.NomineeName,
                    NomineeRelation = customer.NomineeRelation,
                    UserId = customer.UserId,
                    CreatedDate = customer.CreatedDate
                };

                return new ApiResponseDTO<CustomerResponseDTO>
                {
                    Success = true,
                    Message = "Customer retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                _logger.LogError(ex, "LOG-016: Unexpected system error during GetCustomerByUserId.");
                return new ApiResponseDTO<CustomerResponseDTO>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<string>> AddCustomerAsync(
     CustomerRequestDTO request)
        {
            try
            {
                // 1. Check whether the user exists
                var user = await _userRepository.GetByIdAsync(request.UserId);

                if (user == null)
                {
                    throw new ResourceNotFoundException("User not found.");
                }

                // 2. Verify the authenticated user's role and ID
                var currentUser = _httpContextAccessor.HttpContext?.User;

                var currentUserRole =
                    currentUser?.FindFirst(ClaimTypes.Role)?.Value
                    ?? currentUser?.FindFirst("role")?.Value
                    ?? currentUser?.FindFirst(
                        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                    )?.Value;

                if (currentUserRole == "Customer")
                {
                    var currentUserIdClaim =
                        currentUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? currentUser?.FindFirst("nameid")?.Value
                        ?? currentUser?.FindFirst("sub")?.Value;

                    if (!int.TryParse(currentUserIdClaim, out int currentUserId))
                    {
                        return new ApiResponseDTO<string>
                        {
                            Success = false,
                            Message = "Unable to identify the authenticated user."
                        };
                    }

                    if (request.UserId != currentUserId)
                    {
                        _logger.LogWarning(
                            "LOG-014: Customer {UserId} attempted to create a profile for UserId {RequestedUserId}.",
                            currentUserId,
                            request.UserId
                        );

                        throw new ForbiddenAccessException(
                            "You are not authorized to create a profile for this user."
                        );
                    }
                }

                // 3. Check whether a customer profile already exists
                var existingCustomers = await _customerRepository.FindAsync(
                    c => c.UserId == request.UserId
                );

                if (existingCustomers.Any())
                {
                    _logger.LogWarning(
                        "Customer profile already exists for UserId: {UserId}",
                        request.UserId
                    );

                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "A customer profile already exists for this user. Please update the existing profile."
                    };
                }

                var min18Date = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18));
                if (request.DateOfBirth > min18Date)
                {
                    return new ApiResponseDTO<string> { Success = false, Message = "Customer must be at least 18 years old." };
                }

                // 4. Create the customer entity
                var customer = new Customer
                {
                    Gender = request.Gender,
                    DateOfBirth = request.DateOfBirth,
                    City = request.City,
                    Address = request.Address,
                    State = request.State,
                    Pincode = request.Pincode,
                    NomineeName = request.NomineeName,
                    NomineeRelation = request.NomineeRelation,
                    UserId = request.UserId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                // 5. Save the customer
                await _customerRepository.AddAsync(customer);
                await _customerRepository.SaveChangesAsync();

                _logger.LogInformation(
                    "LOG-001: Customer profile created successfully for UserId: {UserId}",
                    request.UserId
                );

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Customer added successfully.",
                        Data = "customer id  is"+ customer.CustomerId.ToString()

                };
            }
            catch (ResourceNotFoundException)
            {
                throw;
            }
            catch (ForbiddenAccessException)
            {
                throw;
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                _logger.LogError(
                    dbEx,
                    "LOG-016: DB constraint error saving customer for UserId: {UserId}. Inner: {Inner}",
                    request.UserId,
                    inner
                );

                // Surface a meaningful message for FK / unique violations
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = $"Database error while saving customer: {inner}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "LOG-016: Error saving customer for UserId: {UserId}. Error: {Error}",
                    request.UserId,
                    ex.Message
                );

                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = $"An error occurred while saving the customer: {ex.Message}"
                };
            }
        }

        public async Task<ApiResponseDTO<string>> UpdateCustomerAsync(int id, CustomerRequestDTO request)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);

                if (customer == null)
                {
                    throw new ResourceNotFoundException("Customer not found.");
                }

                var currentUserRole = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value);
                if (currentUserRole == "Customer")
                {
                    var currentUserIdClaim = (_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("nameid")?.Value ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value);
                    int.TryParse(currentUserIdClaim, out int currentUserId);
                    if (customer.UserId != currentUserId || request.UserId != currentUserId)
                    {
                        _logger.LogWarning("LOG-014: Security failure. Customer {UserId} attempted to update unauthorized customer profile: {CustomerId}", currentUserId, id);
                        throw new ForbiddenAccessException("You are not authorized to update this customer profile.");
                    }
                }

                var min18Date = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18));
                if (request.DateOfBirth > min18Date)
                {
                    return new ApiResponseDTO<string> { Success = false, Message = "Customer must be at least 18 years old." };
                }

                customer.Gender = request.Gender;
                customer.DateOfBirth = request.DateOfBirth;
                customer.City = request.City;
                customer.Address = request.Address;
                customer.State = request.State;
                customer.Pincode = request.Pincode;
                customer.NomineeName = request.NomineeName;
                customer.NomineeRelation = request.NomineeRelation;
                customer.UserId = request.UserId;
                customer.UpdatedDate = DateTime.UtcNow;

                await _customerRepository.UpdateAsync(customer);
                await _customerRepository.SaveChangesAsync();

                _logger.LogInformation("LOG-001: Customer profile updated successfully for CustomerId: {CustomerId}", id);

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Customer updated successfully."
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

        public async Task<ApiResponseDTO<string>> DeleteCustomerAsync(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);

                if (customer == null)
                {
                    throw new ResourceNotFoundException("Customer not found.");
                }

                await _customerRepository.DeleteAsync(id);
                await _customerRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Customer deleted successfully."
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