using Azure;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Application.DTOs.Auth;
using TicketManagement.Application.Interfaces;

namespace TicketManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        //Constructor Injection
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("userRegister")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid) { 
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerDto);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("userLogin")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            //validate input
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Call service
            var response = await _authService.LoginAsync(loginDto);

            //return response
            return StatusCode(response.StatusCode, response);
        }
    }
}
