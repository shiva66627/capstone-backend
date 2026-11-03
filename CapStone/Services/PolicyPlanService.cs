using CapStone.DTO.Common;
using CapStone.DTO.PolicyPlan;
using CapStone.Exceptions;
using CapStone.Models;
using CapStone.Repositories;
using Microsoft.Extensions.Logging;

namespace CapStone.Services
{
    public class PolicyPlanService : IPolicyPlanService
    {
        private readonly IPolicyPlanRepository _planRepository;
        private readonly IInsuranceProductRepository _productRepository;
        private readonly ILogger<PolicyPlanService> _logger;

        public PolicyPlanService(
            IPolicyPlanRepository planRepository,
            IInsuranceProductRepository productRepository,
            ILogger<PolicyPlanService> logger)
        {
            _planRepository = planRepository;
            _productRepository = productRepository;
            _logger = logger;
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
                    ProductName = p.InsuranceProduct?.ProductName ?? string.Empty,
                    ProductType = p.InsuranceProduct != null ? (int)p.InsuranceProduct.ProductType : 0,
                    CoverageAmount = p.CoverageAmount,
                    PremiumAmount = p.PremiumAmount,
                    PremiumType = p.PremiumType,
                    DurationYears = p.DurationYears,
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
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<IEnumerable<PolicyPlanResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<PaginatedResponseDTO<PolicyPlanResponseDTO>>> GetPagedPlansAsync(PlanFilterDTO filter)
        {
            try
            {
                var (plans, totalRecords) = await _planRepository.GetFilteredPlansAsync(filter);

                var result = plans.Select(p => new PolicyPlanResponseDTO
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    ProductName = p.InsuranceProduct?.ProductName ?? string.Empty,
                    ProductType = p.InsuranceProduct != null ? (int)p.InsuranceProduct.ProductType : 0,
                    CoverageAmount = p.CoverageAmount,
                    PremiumAmount = p.PremiumAmount,
                    PremiumType = p.PremiumType,
                    DurationYears = p.DurationYears,
                    PlanStatus = p.PlanStatus,
                    ProductId = p.ProductId,
                    CreatedDate = p.CreatedDate
                });

                var paginatedData = new PaginatedResponseDTO<PolicyPlanResponseDTO>(
                    result, totalRecords, filter.PageNumber, filter.PageSize, filter.SortField, filter.SortDirection);

                return new ApiResponseDTO<PaginatedResponseDTO<PolicyPlanResponseDTO>>
                {
                    Success = true,
                    Message = "Plans retrieved successfully.",
                    Data = paginatedData
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<PaginatedResponseDTO<PolicyPlanResponseDTO>>
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
                    ProductName = p.InsuranceProduct?.ProductName ?? string.Empty,
                    ProductType = p.InsuranceProduct != null ? (int)p.InsuranceProduct.ProductType : 0,
                    CoverageAmount = p.CoverageAmount,
                    PremiumAmount = p.PremiumAmount,
                    PremiumType = p.PremiumType,
                    DurationYears = p.DurationYears,
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
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
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
                    ProductName = p.InsuranceProduct?.ProductName ?? string.Empty,
                    ProductType = p.InsuranceProduct != null ? (int)p.InsuranceProduct.ProductType : 0,
                    CoverageAmount = p.CoverageAmount,
                    PremiumAmount = p.PremiumAmount,
                    PremiumType = p.PremiumType,
                    DurationYears = p.DurationYears,
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
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
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
                    throw new ResourceNotFoundException("Plan not found.");
                }

                return new ApiResponseDTO<PolicyPlanResponseDTO>
                {
                    Success = true,
                    Message = "Plan retrieved successfully.",
                    Data = new PolicyPlanResponseDTO
                    {
                        PlanId = plan.PlanId,
                        PlanName = plan.PlanName,
                        ProductName = plan.InsuranceProduct?.ProductName ?? string.Empty,
                        ProductType = plan.InsuranceProduct != null ? (int)plan.InsuranceProduct.ProductType : 0,
                        CoverageAmount = plan.CoverageAmount,
                        PremiumAmount = plan.PremiumAmount,
                        PremiumType = plan.PremiumType,
                        DurationYears = plan.DurationYears,
                        PlanStatus = plan.PlanStatus,
                        ProductId = plan.ProductId,
                        CreatedDate = plan.CreatedDate
                    }
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
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
                    _logger.LogWarning("LOG-015: Business rule violation. Duplicate plan name creation attempted: {Name}", request.PlanName);
                    throw new BusinessRuleViolationException("Plan name already exists.");
                }

                if (request.CoverageAmount <= request.PremiumAmount)
                {
                    _logger.LogWarning("LOG-015: Business rule violation. Coverage amount {Coverage} must be greater than premium amount {Premium}", request.CoverageAmount, request.PremiumAmount);
                    throw new BusinessRuleViolationException("Coverage amount must be greater than premium amount.");
                }

                var product = await _productRepository.GetByIdAsync(request.ProductId);

                if (product == null)
                {
                    throw new ResourceNotFoundException("Insurance product not found.");
                }

                if (product.Status != ActiveStatus.Available)
                {
                    _logger.LogWarning("LOG-015: Business rule violation. Attempted to create plan under inactive product: {ProductId}", request.ProductId);
                    throw new BusinessRuleViolationException("Insurance product is not available.");
                }

                var plan = new PolicyPlan
                {
                    PlanName = request.PlanName.Trim(),
                    CoverageAmount = request.CoverageAmount,
                    PremiumAmount = request.PremiumAmount,
                    PremiumType = request.PremiumType,
                    DurationYears = request.DurationYears,
                    PlanStatus = request.PlanStatus,
                    ProductId = request.ProductId,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                await _planRepository.AddAsync(plan);
                await _planRepository.SaveChangesAsync();

                _logger.LogInformation("LOG-005: Plan creation successful. Plan: {Name}, ProductId: {ProductId}", request.PlanName, request.ProductId);

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Policy plan created successfully."
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                _logger.LogError(ex, "LOG-016: Unexpected system error during Plan creation.");
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
                    throw new ResourceNotFoundException("Policy plan not found.");
                }

                var existing = await _planRepository.GetByPlanNameAsync(request.PlanName.Trim());

                if (existing != null && existing.PlanId != id)
                {
                    _logger.LogWarning("LOG-015: Business rule violation. Duplicate plan name update attempted: {Name}", request.PlanName);
                    throw new BusinessRuleViolationException("Plan name already exists.");
                }

                if (request.CoverageAmount <= request.PremiumAmount)
                {
                    _logger.LogWarning("LOG-015: Business rule violation. Coverage amount {Coverage} must be greater than premium amount {Premium}", request.CoverageAmount, request.PremiumAmount);
                    throw new BusinessRuleViolationException("Coverage amount must be greater than premium amount.");
                }

                var product = await _productRepository.GetByIdAsync(request.ProductId);

                if (product == null)
                {
                    throw new ResourceNotFoundException("Insurance product not found.");
                }

                if (product.Status != ActiveStatus.Available)
                {
                    _logger.LogWarning("LOG-015: Business rule violation. Attempted to update plan under inactive product: {ProductId}", request.ProductId);
                    throw new BusinessRuleViolationException("Insurance product is not available.");
                }

                plan.PlanName = request.PlanName.Trim();
                plan.CoverageAmount = request.CoverageAmount;
                plan.PremiumAmount = request.PremiumAmount;
                plan.PremiumType = request.PremiumType;
                plan.DurationYears = request.DurationYears;
                plan.PlanStatus = request.PlanStatus;
                plan.ProductId = request.ProductId;
                plan.UpdatedDate = DateTime.UtcNow;

                await _planRepository.UpdateAsync(plan);
                await _planRepository.SaveChangesAsync();

                _logger.LogInformation("LOG-005: Plan update successful. PlanId: {PlanId}, Plan: {Name}", id, request.PlanName);

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Policy plan updated successfully."
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                _logger.LogError(ex, "LOG-016: Unexpected system error during Plan update.");
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
                    throw new ResourceNotFoundException("Policy plan not found.");
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
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ApiResponseDTO<string>> ActivatePlanAsync(int id)
        {
            try
            {
                var plan = await _planRepository.GetByIdAsync(id);

                if (plan == null)
                {
                    throw new ResourceNotFoundException("Policy plan not found.");
                }

                plan.PlanStatus = PlanStatus.Available;
                plan.UpdatedDate = DateTime.UtcNow;

                await _planRepository.UpdateAsync(plan);
                await _planRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Policy plan activated successfully."
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