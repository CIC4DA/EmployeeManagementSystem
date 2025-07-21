using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]

    public class AuthenticationController(IUserAccount accountInterface) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> CreateAsync(Register User)
        {
            if (User == null) return BadRequest("Model is empty");
            var result = await accountInterface.CreateAsync(User);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> SignInAsync(Login User)
        {
            if (User == null) return BadRequest("Model is empty");
            var result = await accountInterface.SignInAsync(User);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshToken Token)
        {
            if (Token == null) return BadRequest("Model is empty");
            var result = await accountInterface.RefreshTokenAsync(Token);
            return Ok(result);
        }
    }
}