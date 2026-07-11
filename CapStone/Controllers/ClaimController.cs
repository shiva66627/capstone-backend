using CapStone.DTO.Claim;
using CapStone.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapStone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ClaimController : ControllerBase
    {
        private readonly IClaimService _claimService;

        public ClaimController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        // GET: api/Claim
        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllClaims()
        {
            var result = await _claimService.GetAllClaimsAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // GET: api/Claim/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClaimById(int id)
        {
            var result = await _claimService.GetClaimByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        // GET: api/Claim/customer/1
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetClaimsByCustomer(int customerId)
        {
            var result = await _claimService.GetClaimsByCustomerAsync(customerId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // POST: api/Claim/submit
        [HttpPost("submit")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Customer")]
        public async Task<IActionResult> SubmitClaim(ClaimRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _claimService.SubmitClaimAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // PUT: api/Claim/review
        [HttpPut("review")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Staff")]
        public async Task<IActionResult> ReviewClaim(ClaimReviewDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _claimService.ReviewClaimAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // PUT: api/Claim/decision
        [HttpPut("decision")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> DecideClaim(ClaimDecisionDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _claimService.DecideClaimAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}