using CapStone.DTO.Common;
using CapStone.DTO.Policy;
using CapStone.Models;
using CapStone.Repositories;

namespace CapStone.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly IPolicyRepository _policyRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPolicyPlanRepository _planRepository;

        public PolicyService(
            IPolicyRepository policyRepository,
            ICustomerRepository customerRepository,
            IPolicyPlanRepository planRepository)
        {
            _policyRepository = policyRepository;
            _customerRepository = customerRepository;
            _planRepository = planRepository;
        }

        public async Task<ApiResponseDTO<IEnumerable<PolicyResponseDTO>>> GetAllPoliciesAsync()
        {
            try
            {
                var policies = await _policyRepository.GetAllAsync();

                var result = policies.Select(p => new PolicyResponseDTO
                {
                    PolicyId = p.PolicyId,
                    PolicyNumber = p.PolicyNumber,
                    PremiumPaid = p.PremiumPaid,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    PolicyStatus = p.PolicyStatus,
                    CustomerId = p.CustomerId,
                    PlanId = p.PlanId,
                    CreatedDate = p.CreatedDate
                });

                return new ApiResponseDTO<IEnumerable<PolicyResponseDTO>>
                {
                    Success = true,
                    Message = "Policies retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<IEnumerable<PolicyResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<PolicyResponseDTO>> GetPolicyByIdAsync(int id)
        {
            try
            {
                var policy = await _policyRepository.GetByIdAsync(id);

                if (policy == null)
                {
                    return new ApiResponseDTO<PolicyResponseDTO>
                    {
                        Success = false,
                        Message = "Policy not found."
                    };
                }

                return new ApiResponseDTO<PolicyResponseDTO>
                {
                    Success = true,
                    Message = "Policy retrieved successfully.",
                    Data = new PolicyResponseDTO
                    {
                        PolicyId = policy.PolicyId,
                        PolicyNumber = policy.PolicyNumber,
                        PremiumPaid = policy.PremiumPaid,
                        StartDate = policy.StartDate,
                        EndDate = policy.EndDate,
                        PolicyStatus = policy.PolicyStatus,
                        CustomerId = policy.CustomerId,
                        PlanId = policy.PlanId,
                        CreatedDate = policy.CreatedDate
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<PolicyResponseDTO>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<IEnumerable<PolicyResponseDTO>>> GetPoliciesByCustomerAsync(int customerId)
        {
            try
            {
                var policies = await _policyRepository.GetPoliciesByCustomerAsync(customerId);

                var result = policies.Select(p => new PolicyResponseDTO
                {
                    PolicyId = p.PolicyId,
                    PolicyNumber = p.PolicyNumber,
                    PremiumPaid = p.PremiumPaid,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    PolicyStatus = p.PolicyStatus,
                    CustomerId = p.CustomerId,
                    PlanId = p.PlanId,
                    CreatedDate = p.CreatedDate
                });

                return new ApiResponseDTO<IEnumerable<PolicyResponseDTO>>
                {
                    Success = true,
                    Message = "Customer policies retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<IEnumerable<PolicyResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ApiResponseDTO<string>> PurchasePolicyAsync(PurchasePolicyRequestDTO request)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(request.CustomerId);

                if (customer == null)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Customer not found."
                    };
                }

                var plan = await _planRepository.GetByIdAsync(request.PlanId);

                if (plan == null)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Policy plan not found."
                    };
                }

                var random = new Random();
                int policyNumber;

                do
                {
                    policyNumber = random.Next(100000, 999999);
                }
                while (await _policyRepository.PolicyExistsAsync(policyNumber));

                var policy = new Policy
                {
                    PolicyNumber = policyNumber,
                    CustomerId = request.CustomerId,
                    PlanId = request.PlanId,
                    PremiumPaid = 0,
                    StartDate = request.StartDate,
                    EndDate = request.StartDate.AddYears(plan.DurationYears),
                    PolicyStatus = PolicyStatus.Pending,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                await _policyRepository.AddAsync(policy);
                await _policyRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Policy purchased successfully."
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
        public async Task<ApiResponseDTO<string>> IssuePolicyAsync(IssuePolicyRequestDTO request)
        {
            try
            {
                if (await _policyRepository.PolicyExistsAsync(request.PolicyNumber))
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Policy number already exists."
                    };
                }

                var customer = await _customerRepository.GetByIdAsync(request.CustomerId);

                if (customer == null)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Customer not found."
                    };
                }

                var plan = await _planRepository.GetByIdAsync(request.PlanId);

                if (plan == null)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Policy plan not found."
                    };
                }

                var policy = new Policy
                {
                    PolicyNumber = request.PolicyNumber,
                    CustomerId = request.CustomerId,
                    PlanId = request.PlanId,
                    PremiumPaid = request.PremiumPaid,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    PolicyStatus = request.PolicyStatus,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                await _policyRepository.AddAsync(policy);
                await _policyRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Policy issued successfully."
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
        public async Task<ApiResponseDTO<string>> CancelPolicyAsync(int id)
        {
            try
            {
                var policy = await _policyRepository.GetByIdAsync(id);

                if (policy == null)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Policy not found."
                    };
                }

                policy.PolicyStatus = PolicyStatus.Cancelled;
                policy.UpdatedDate = DateTime.UtcNow;

                await _policyRepository.UpdateAsync(policy);
                await _policyRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Policy cancelled successfully."
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