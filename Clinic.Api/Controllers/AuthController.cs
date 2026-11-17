using Clinic.Application.DTOs.Authentication;
using Clinic.Application.Services.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            try
            {
                var token = await _authService.LoginAsync(request);

                return Ok(new
                {
                    AccessToken = token
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new
                {
                    Message = "Invalid email or password."
                });
            }
        }
    }
}
