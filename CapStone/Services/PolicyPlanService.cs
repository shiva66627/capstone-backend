using CapStone.DTO.Common;
using CapStone.DTO.PolicyPlan;
using CapStone.Models;
using CapStone.Repositories;

namespace CapStone.Services
{
    public class PolicyPlanService : IPolicyPlanService
    {
        private readonly IPolicyPlanRepository _planRepository;
        private readonly IInsuranceProductRepository _productRepository;

        public PolicyPlanService(
            IPolicyPlanRepository planRepository,
            IInsuranceProductRepository productRepository)
        {
            _planRepository = planRepository;
            _productRepository = productRepository;
        }

        public async Task<ApiResponseDTO<IEnumerable<PolicyPlanResponseDTO>>> GetAllPlansAsync()
        {
            try
            {
                var plans = await _planRepository.GetAllAsync();

                var result = plans.Select(p => new PolicyPlanResponseDTO
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    CoverageAmount = p.CoverageAmount,
                    PremiumAmount = p.PremiumAmount,
                    PremiumType = p.PremiumType,
                    DurationYears = p.DurationYears,
                    TermsConditions = p.TermsConditions,
                    PlanStatus = p.PlanStatus,
                    ProductId = p.ProductId,
                    CreatedDate = p.CreatedDate
                });

                return new ApiResponseDTO<IEnumerable<PolicyPlanResponseDTO>>
                {
                    Success = true,
                    Message = "Plans retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<IEnumerable<PolicyPlanResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<IEnumerable<PolicyPlanResponseDTO>>> GetAvailablePlansAsync()
        {
            try
            {
                var plans = await _planRepository.GetAvailablePlansAsync();

                var result = plans.Select(p => new PolicyPlanResponseDTO
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    CoverageAmount = p.CoverageAmount,
                    PremiumAmount = p.PremiumAmount,
                    PremiumType = p.PremiumType,
                    DurationYears = p.DurationYears,
                    TermsConditions = p.TermsConditions,
                    PlanStatus = p.PlanStatus,
                    ProductId = p.ProductId,
                    CreatedDate = p.CreatedDate
                });

                return new ApiResponseDTO<IEnumerable<PolicyPlanResponseDTO>>
                {
                    Success = true,
                    Message = "Available plans retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<IEnumerable<PolicyPlanResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<IEnumerable<PolicyPlanResponseDTO>>> GetPlansByProductAsync(int productId)
        {
            try
            {
                var plans = await _planRepository.GetPlansByProductAsync(productId);

                var result = plans.Select(p => new PolicyPlanResponseDTO
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    CoverageAmount = p.CoverageAmount,
                    PremiumAmount = p.PremiumAmount,
                    PremiumType = p.PremiumType,
                    DurationYears = p.DurationYears,
                    TermsConditions = p.TermsConditions,
                    PlanStatus = p.PlanStatus,
                    ProductId = p.ProductId,
                    CreatedDate = p.CreatedDate
                });

                return new ApiResponseDTO<IEnumerable<PolicyPlanResponseDTO>>
                {
                    Success = true,
                    Message = "Plans retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<IEnumerable<PolicyPlanResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<PolicyPlanResponseDTO>> GetPlanByIdAsync(int id)
        {
            try
            {
                var plan = await _planRepository.GetByIdAsync(id);

                if (plan == null)
                {
                    return new ApiResponseDTO<PolicyPlanResponseDTO>
                    {
                        Success = false,
                        Message = "Plan not found."
                    };
                }

                return new ApiResponseDTO<PolicyPlanResponseDTO>
                {
                    Success = true,
                    Message = "Plan retrieved successfully.",
                    Data = new PolicyPlanResponseDTO
                    {
                        PlanId = plan.PlanId,
                        PlanName = plan.PlanName,
                        CoverageAmount = plan.CoverageAmount,
                        PremiumAmount = plan.PremiumAmount,
                        PremiumType = plan.PremiumType,
                        DurationYears = plan.DurationYears,
                        TermsConditions = plan.TermsConditions,
                        PlanStatus = plan.PlanStatus,
                        ProductId = plan.ProductId,
                        CreatedDate = plan.CreatedDate
                    }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<PolicyPlanResponseDTO>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<string>> AddPlanAsync(PolicyPlanRequestDTO request)
        {
            try
            {
                var existing = await _planRepository.GetByPlanNameAsync(request.PlanName.Trim());

                if (existing != null)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Plan name already exists."
                    };
                }

                if (request.CoverageAmount <= request.PremiumAmount)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Coverage amount must be greater than premium amount."
                    };
                }

                var product = await _productRepository.GetByIdAsync(request.ProductId);

                if (product == null)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Insurance product not found."
                    };
                }

                if (product.Status != ActiveStatus.Available)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Insurance product is not available."
                    };
                }

                var plan = new PolicyPlan
                {
                    PlanName = request.PlanName.Trim(),
                    CoverageAmount = request.CoverageAmount,
                    PremiumAmount = request.PremiumAmount,
                    PremiumType = request.PremiumType,
                    DurationYears = request.DurationYears,
                    TermsConditions = request.TermsConditions.Trim(),
                    PlanStatus = request.PlanStatus,
                    ProductId = request.ProductId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                await _planRepository.AddAsync(plan);
                await _planRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Policy plan created successfully."
                };
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = errorMessage
                };
            }
        }

        public async Task<ApiResponseDTO<string>> UpdatePlanAsync(int id, PolicyPlanRequestDTO request)
        {
            try
            {
                var plan = await _planRepository.GetByIdAsync(id);

                if (plan == null)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Policy plan not found."
                    };
                }

                var existing = await _planRepository.GetByPlanNameAsync(request.PlanName.Trim());

                if (existing != null && existing.PlanId != id)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Plan name already exists."
                    };
                }

                if (request.CoverageAmount <= request.PremiumAmount)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Coverage amount must be greater than premium amount."
                    };
                }

                var product = await _productRepository.GetByIdAsync(request.ProductId);

                if (product == null)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Insurance product not found."
                    };
                }

                if (product.Status != ActiveStatus.Available)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Insurance product is not available."
                    };
                }

                plan.PlanName = request.PlanName.Trim();
                plan.CoverageAmount = request.CoverageAmount;
                plan.PremiumAmount = request.PremiumAmount;
                plan.PremiumType = request.PremiumType;
                plan.DurationYears = request.DurationYears;
                plan.TermsConditions = request.TermsConditions.Trim();
                plan.PlanStatus = request.PlanStatus;
                plan.ProductId = request.ProductId;
                plan.UpdatedDate = DateTime.UtcNow;

                await _planRepository.UpdateAsync(plan);
                await _planRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Policy plan updated successfully."
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
        public async Task<ApiResponseDTO<string>> DeactivatePlanAsync(int id)
        {
            try
            {
                var plan = await _planRepository.GetByIdAsync(id);

                if (plan == null)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Policy plan not found."
                    };
                }

                plan.PlanStatus = PlanStatus.NotAvailable;
                plan.UpdatedDate = DateTime.UtcNow;

                await _planRepository.UpdateAsync(plan);
                await _planRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Policy plan deactivated successfully."
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