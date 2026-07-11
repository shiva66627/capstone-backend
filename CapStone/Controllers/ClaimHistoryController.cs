using CapStone.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapStone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ClaimHistoryController : ControllerBase
    {
        private readonly IClaimHistoryService _historyService;

        public ClaimHistoryController(IClaimHistoryService historyService)
        {
            _historyService = historyService;
        }

        // GET: api/ClaimHistory
        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllHistory()
        {
            var result = await _historyService.GetAllHistoryAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // GET: api/ClaimHistory/claim/5
        [HttpGet("claim/{claimId}")]
        public async Task<IActionResult> GetHistoryByClaim(int claimId)
        {
            var result = await _historyService.GetHistoryByClaimAsync(claimId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}