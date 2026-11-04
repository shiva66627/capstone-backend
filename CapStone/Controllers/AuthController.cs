using CapStone.DTO.Authentication;
using CapStone.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapStone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(
         [FromBody] VerifyOtpRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.VerifyOtpAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        // resend otp
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp(
    [FromBody] ResendOtpRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ResendOtpAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(request);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        //[HttpPost("admin-login")]
        //public async Task<IActionResult> AdminLogin(LoginRequestDTO request)
        //{
        //    var result = await _authService.AdminLoginAsync(request);

        //    if (!result.Success)
        //        return Unauthorized(result);

        //    return Ok(result);
        //}

        // POST: api/Auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ForgotPasswordAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // POST: api/Auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ResetPasswordAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}