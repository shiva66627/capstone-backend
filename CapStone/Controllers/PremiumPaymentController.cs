using CapStone.DTO.PremiumPayment;
using CapStone.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapStone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class PremiumPaymentController : ControllerBase
    {
        private readonly IPremiumPaymentService _paymentService;

        public PremiumPaymentController(IPremiumPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // GET: api/PremiumPayment
        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllPayments()
        {
            var result = await _paymentService.GetAllPaymentsAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // GET: api/PremiumPayment/paged
        [HttpGet("paged")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetPagedPayments([FromQuery] PaymentFilterDTO filter)
        {
            var result = await _paymentService.GetPagedPaymentsAsync(filter);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // GET: api/PremiumPayment/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var result = await _paymentService.GetPaymentByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        // GET: api/PremiumPayment/policy/5
        [HttpGet("policy/{policyId}")]
        public async Task<IActionResult> GetPaymentsByPolicy(int policyId)
        {
            var result = await _paymentService.GetPaymentsByPolicyAsync(policyId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // GET: api/PremiumPayment/customer/5
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetPaymentsByCustomer(int customerId)
        {
            var result = await _paymentService.GetPaymentsByCustomerAsync(customerId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // POST: api/PremiumPayment
        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Customer")]
        public async Task<IActionResult> MakePayment(PremiumPaymentRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentService.MakePaymentAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // GET: api/PremiumPayment/next-due/5
        [HttpGet("next-due/{policyId}")]
        public async Task<IActionResult> GetNextPremiumPayment(int policyId)
        {
            var result = await _paymentService.GetNextPremiumPaymentDetailsAsync(policyId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}