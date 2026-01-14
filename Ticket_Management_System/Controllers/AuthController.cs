using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketManagement.Application.DTOs.Auth;
using TicketManagement.Application.Interfaces;
using TicketManagement.Application.Services;

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

     
    //Method to logout the user
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // Get logged-in userId from JWT
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // Call service
        var response = await _authService.LogoutAsync(userId);

        // Return standard response
        return StatusCode(response.StatusCode, response);
    }

        //Method for refresh token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto dto)
        {
            var response = await _authService.RefreshTokenAsync(dto);
            return StatusCode(response.StatusCode, response);
        }


    }
}
