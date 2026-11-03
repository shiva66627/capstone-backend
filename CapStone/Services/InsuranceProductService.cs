using CapStone.DTO.Common;
using CapStone.DTO.InsuranceProduct;
using CapStone.Exceptions;
using CapStone.Models;
using CapStone.Repositories;
using Microsoft.Extensions.Logging;

namespace CapStone.Services
{
    public class InsuranceProductService : IInsuranceProductService
    {
        private readonly IInsuranceProductRepository _productRepository;
        private readonly ILogger<InsuranceProductService> _logger;

        public InsuranceProductService(
            IInsuranceProductRepository productRepository,
            ILogger<InsuranceProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ApiResponseDTO<IEnumerable<InsuranceProductResponseDTO>>> GetAllProductsAsync()
        {
            try
            {
                var products = await _productRepository.GetAllAsync();

                var result = products.Select(p => new InsuranceProductResponseDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductType = p.ProductType,
                    Description = p.Description,
                    Status = p.Status,
                    CreatedDate = p.CreatedDate
                });

                return new ApiResponseDTO<IEnumerable<InsuranceProductResponseDTO>>
                {
                    Success = true,
                    Message = "Products retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<IEnumerable<InsuranceProductResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<PaginatedResponseDTO<InsuranceProductResponseDTO>>> GetPagedProductsAsync(ProductFilterDTO filter)
        {
            try
            {
                var (products, totalRecords) = await _productRepository.GetFilteredProductsAsync(filter);

                var result = products.Select(p => new InsuranceProductResponseDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductType = p.ProductType,
                    Description = p.Description,
                    Status = p.Status,
                    CreatedDate = p.CreatedDate
                });

                var paginatedData = new PaginatedResponseDTO<InsuranceProductResponseDTO>(
                    result, totalRecords, filter.PageNumber, filter.PageSize, filter.SortField, filter.SortDirection);

                return new ApiResponseDTO<PaginatedResponseDTO<InsuranceProductResponseDTO>>
                {
                    Success = true,
                    Message = "Products retrieved successfully.",
                    Data = paginatedData
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<PaginatedResponseDTO<InsuranceProductResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<IEnumerable<InsuranceProductResponseDTO>>> GetAvailableProductsAsync()
        {
            try
            {
                var products = await _productRepository.GetAvailableProductsAsync();

                var result = products.Select(p => new InsuranceProductResponseDTO
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductType = p.ProductType,
                    Description = p.Description,
                    Status = p.Status,
                    CreatedDate = p.CreatedDate
                });

                return new ApiResponseDTO<IEnumerable<InsuranceProductResponseDTO>>
                {
                    Success = true,
                    Message = "Available products retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<IEnumerable<InsuranceProductResponseDTO>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<InsuranceProductResponseDTO>> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);

                if (product == null)
                {
                    throw new ResourceNotFoundException("Product not found.");
                }

                var result = new InsuranceProductResponseDTO
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductType = product.ProductType,
                    Description = product.Description,
                    Status = product.Status,
                    CreatedDate = product.CreatedDate
                };

                return new ApiResponseDTO<InsuranceProductResponseDTO>
                {
                    Success = true,
                    Message = "Product retrieved successfully.",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                return new ApiResponseDTO<InsuranceProductResponseDTO>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<string>> AddProductAsync(InsuranceProductRequestDTO request)
        {
            try
            {
                if (await _productRepository.ProductExistsAsync(request.ProductName.Trim()))
                {
                    _logger.LogWarning("LOG-015: Business rule violation. Duplicate product name creation attempted: {Name}", request.ProductName);
                    throw new DuplicateProductNameException("Product already exists.");
                }

                var product = new InsuranceProduct
                {
                    ProductName = request.ProductName.Trim(),
                    ProductType = request.ProductType,
                    Description = request.Description.Trim(),
                    Status = request.Status,
                    CreatedDate = DateTime.UtcNow,
                    
                };

                await _productRepository.AddAsync(product);
                await _productRepository.SaveChangesAsync();

                _logger.LogInformation("LOG-004: Product creation successful. Product: {Name}, Type: {Type}", request.ProductName, request.ProductType);

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Product added successfully."
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                _logger.LogError(ex, "LOG-016: Unexpected system error during Product creation.");
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<string>> UpdateProductAsync(int id, InsuranceProductRequestDTO request)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);

                if (product == null)
                {
                    throw new ResourceNotFoundException("Product not found.");
                }

                var existing = await _productRepository.GetByProductNameAsync(request.ProductName.Trim());

                if (existing != null && existing.ProductId != id)
                {
                    _logger.LogWarning("LOG-015: Business rule violation. Duplicate product name update attempted: {Name}", request.ProductName);
                    throw new DuplicateProductNameException("Product name already exists.");
                }

                product.ProductName = request.ProductName.Trim();
                product.ProductType = request.ProductType;
                product.Description = request.Description.Trim();
                product.Status = request.Status;

                await _productRepository.UpdateAsync(product);
                await _productRepository.SaveChangesAsync();

                _logger.LogInformation("LOG-004: Product update successful. ProductId: {Id}, Product: {Name}", id, request.ProductName);

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Product updated successfully."
                };
            }
            catch (Exception ex)
            {
                if (ex.GetType().Namespace == "CapStone.Exceptions") throw;
                _logger.LogError(ex, "LOG-016: Unexpected system error during Product update.");
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<string>> DeactivateProductAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);

                if (product == null)
                {
                    throw new ResourceNotFoundException("Product not found.");
                }

                product.Status = ActiveStatus.NotAvailable;

                await _productRepository.UpdateAsync(product);
                await _productRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Product deactivated successfully."
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
        public async Task<ApiResponseDTO<string>> ActivateProductAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);

                if (product == null)
                {
                    throw new ResourceNotFoundException("Product not found.");
                }

                product.Status = ActiveStatus.Available;

                await _productRepository.UpdateAsync(product);
                await _productRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Product activated successfully."
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