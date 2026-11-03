using CapStone.DTO.Customer;
using CapStone.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapStone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: api/Customer
        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _customerService.GetAllCustomersAsync();

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // GET: api/Customer/paged
        [HttpGet("paged")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetPagedCustomers([FromQuery] CustomerFilterDTO filter)
        {
            var result = await _customerService.GetPagedCustomersAsync(filter);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // GET: api/Customer/5
        [HttpGet("{id}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var result = await _customerService.GetCustomerByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        // GET: api/Customer/user/5
        [HttpGet("user/{userId}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCustomerByUserId(int userId)
        {
            var result = await _customerService.GetCustomerByUserIdAsync(userId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

      //  POST: api/Customer
       [HttpPost]
       [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _customerService.AddCustomerAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        [HttpPut("{id}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _customerService.UpdateCustomerAsync(id, request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // DELETE: api/Customer/5
        [HttpDelete("{id}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}