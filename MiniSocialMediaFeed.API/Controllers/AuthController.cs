using Microsoft.AspNetCore.Mvc;
using MiniSocialMediaFeed.Application.Dtos.RequestDto;
using MiniSocialMediaFeed.Application.Interfaces;

namespace MiniSocialMediaFeed.API.Controllers
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserReqDto registerUserDto)
        {
            var response = await _authService.RegisterUserAsync(registerUserDto);
            if (!response.Status)
            {
                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserReqDto loginUserDto)
        {
            var response = await _authService.LoginUserAsync(loginUserDto);
            if (!response.Status)
            {
                return StatusCode(response.StatusCode, response);
            }

            return StatusCode(response.StatusCode, response);
        }
    }
}
