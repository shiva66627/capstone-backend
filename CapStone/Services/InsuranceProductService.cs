using CapStone.DTO.Common;
using CapStone.DTO.InsuranceProduct;
using CapStone.Models;
using CapStone.Repositories;

namespace CapStone.Services
{
    public class InsuranceProductService : IInsuranceProductService
    {
        private readonly IInsuranceProductRepository _productRepository;

        public InsuranceProductService(IInsuranceProductRepository productRepository)
        {
            _productRepository = productRepository;
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
                return new ApiResponseDTO<IEnumerable<InsuranceProductResponseDTO>>
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
                    return new ApiResponseDTO<InsuranceProductResponseDTO>
                    {
                        Success = false,
                        Message = "Product not found."
                    };
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
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Product already exists."
                    };
                }

                var product = new InsuranceProduct
                {
                    ProductName = request.ProductName.Trim(),
                    ProductType = request.ProductType,
                    Description = request.Description.Trim(),
                    Status = request.Status,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                await _productRepository.AddAsync(product);
                await _productRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Product added successfully."
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

        public async Task<ApiResponseDTO<string>> UpdateProductAsync(int id, InsuranceProductRequestDTO request)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);

                if (product == null)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Product not found."
                    };
                }

                var existing = await _productRepository.GetByProductNameAsync(request.ProductName.Trim());

                if (existing != null && existing.ProductId != id)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Product name already exists."
                    };
                }

                product.ProductName = request.ProductName.Trim();
                product.ProductType = request.ProductType;
                product.Description = request.Description.Trim();
                product.Status = request.Status;
                product.UpdatedDate = DateTime.UtcNow;

                await _productRepository.UpdateAsync(product);
                await _productRepository.SaveChangesAsync();

                return new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Product updated successfully."
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

        public async Task<ApiResponseDTO<string>> DeactivateProductAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);

                if (product == null)
                {
                    return new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Product not found."
                    };
                }

                product.Status = ActiveStatus.NotAvailable;
                product.UpdatedDate = DateTime.UtcNow;

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
                return new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}