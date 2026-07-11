using CapStone.DTO.PolicyPlan;
using CapStone.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapStone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class PolicyPlanController : ControllerBase
    {
        private readonly IPolicyPlanService _planService;

        public PolicyPlanController(IPolicyPlanService planService)
        {
            _planService = planService;
        }

        // GET: api/PolicyPlan
        [HttpGet]
    
        public async Task<IActionResult> GetAllPlans()
        {
            var result = await _planService.GetAllPlansAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // GET: api/PolicyPlan/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanById(int id)
        {
            var result = await _planService.GetPlanByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        // GET: api/PolicyPlan/available
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailablePlans()
        {
            var result = await _planService.GetAvailablePlansAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // GET: api/PolicyPlan/product/1
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetPlansByProduct(int productId)
        {
            var result = await _planService.GetPlansByProductAsync(productId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // POST: api/PolicyPlan
        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPlan([FromBody] PolicyPlanRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _planService.AddPlanAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // PUT: api/PolicyPlan/5
        [HttpPut("{id}")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePlan(int id, [FromBody] PolicyPlanRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _planService.UpdatePlanAsync(id, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // PUT: api/PolicyPlan/5/deactivate
        [HttpPut("{id}/deactivate")]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivatePlan(int id)
        {
            var result = await _planService.DeactivatePlanAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}