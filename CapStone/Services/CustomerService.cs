using CapStone.DTO.Common;
using CapStone.DTO.Customer;
using CapStone.Models;
using CapStone.Repositories;

namespace CapStone.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;

        public CustomerService(
            ICustomerRepository customerRepository,
            IUserRepository userRepository)
        {
            _customerRepository = customerRepository;
            _userRepository = userRepository;
        }

        public async Task<ApiResponseDTO<IEnumerable<CustomerResponseDTO>>> GetAllCustomersAsync()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();

                var result = customers.Select(c => new CustomerResponseDTO
                {
                    CustomerId = c.CustomerId,
                    DateOfBirth = c.DateOfBirth,
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
                return new ApiResponseDTO<IEnumerable<CustomerResponseDTO>>
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
                    return new ApiResponseDTO<CustomerResponseDTO>
                    {
                        Success = false,
                        Message = "Customer not found."
                    };
                }

                var result = new CustomerResponseDTO
                {
                    CustomerId = customer.CustomerId,
                    DateOfBirth = customer.DateOfBirth,
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
                return new ApiResponseDTO<CustomerResponseDTO>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<string>> AddCustomerAsync(CustomerRequestDTO request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId);

                if (user == null)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                var customer = new Customer
                {
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

                await _customerRepository.AddAsync(customer);
                await _customerRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Customer added successfully."
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

        public async Task<ApiResponseDTO<string>> UpdateCustomerAsync(int id, CustomerRequestDTO request)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);

                if (customer == null)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Customer not found."
                    };
                }

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

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Customer updated successfully."
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

        public async Task<ApiResponseDTO<string>> DeleteCustomerAsync(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);

                if (customer == null)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Customer not found."
                    };
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
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}