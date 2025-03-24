using Microsoft.AspNetCore.Mvc;
using SalesApp.DTOs.AuthDTOs;
using SalesApp.Services.Interfaces;

namespace SalesApp.Controllers
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
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> AuthenticateUser(LoginDTO loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            if (response == null)
                return Unauthorized();
            return Ok(response);
        }
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> RegisterNewUser(RegisterDTO registerDto)
        {
            var response = await _authService.RegisterAsync(registerDto);
            if (response == null)
                return BadRequest("Erro ao registrar usuário");
            return Ok(response);
        }
    }

}
