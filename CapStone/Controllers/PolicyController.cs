using CapStone.DTO.Policy;
using CapStone.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapStone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class PolicyController : ControllerBase
    {
        private readonly IPolicyService _policyService;

        public PolicyController(IPolicyService policyService)
        {
            _policyService = policyService;
        }

        // GET: api/Policy
        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllPolicies()
        {
            var result = await _policyService.GetAllPoliciesAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // GET: api/Policy/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPolicyById(int id)
        {
            var result = await _policyService.GetPolicyByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        // GET: api/Policy/customer/1
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetPoliciesByCustomer(int customerId)
        {
            var result = await _policyService.GetPoliciesByCustomerAsync(customerId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // POST: api/Policy/purchase
        [HttpPost("purchase")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Customer")]
        public async Task<IActionResult> PurchasePolicy([FromBody] PurchasePolicyRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _policyService.PurchasePolicyAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // POST: api/Policy/issue
        [HttpPost("issue")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> IssuePolicy([FromBody] IssuePolicyRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _policyService.IssuePolicyAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // PUT: api/Policy/5/cancel
        [HttpPut("{id}/cancel")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CancelPolicy(int id)
        {
            var result = await _policyService.CancelPolicyAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
