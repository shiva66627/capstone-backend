using CapStone.DTO.InsuranceProduct;
using CapStone.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapStone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class InsuranceProductController : ControllerBase
    {
        private readonly IInsuranceProductService _productService;

        public InsuranceProductController(IInsuranceProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Staff,Customer")]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productService.GetAllProductsAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("paged")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetPagedProducts([FromQuery] ProductFilterDTO filter)
        {
            var result = await _productService.GetPagedProductsAsync(filter);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableProducts()
        {
            var result = await _productService.GetAvailableProductsAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct(InsuranceProductRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.AddProductAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, InsuranceProductRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.UpdateProductAsync(id, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("{id}/deactivate")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateProduct(int id)
        {
            var result = await _productService.DeactivateProductAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPut("{id}/activate")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateProduct(int id)
        {
            var result = await _productService.ActivateProductAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}