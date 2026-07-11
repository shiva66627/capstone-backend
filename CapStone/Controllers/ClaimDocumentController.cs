using CapStone.DTO.ClaimDocument;
using CapStone.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapStone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ClaimDocumentController : ControllerBase
    {
        private readonly IClaimDocumentService _documentService;

        public ClaimDocumentController(IClaimDocumentService documentService)
        {
            _documentService = documentService;
        }

        // GET: api/ClaimDocument/5
        [HttpGet("{claimId}")]
        public async Task<IActionResult> GetDocumentsByClaim(int claimId)
        {
            var result = await _documentService.GetDocumentsByClaimAsync(claimId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        // POST: api/ClaimDocument
        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Customer")]
        public async Task<IActionResult> UploadDocument(ClaimDocumentRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _documentService.UploadDocumentAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}