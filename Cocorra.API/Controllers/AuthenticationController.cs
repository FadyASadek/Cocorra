using Cocorra.BLL.DTOS.Auth;
using Cocorra.BLL.Services.Auth;
using Cocorra.DAL.AppMetaData;
using Microsoft.AspNetCore.Mvc;

namespace Cocorra.API.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IAuthServices _authServices;
        public AuthenticationController(IAuthServices authServices)
        {
            _authServices = authServices;
        }
        [HttpPost(Router.AuthenticationRouting.Register)]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authServices.RegisterAsync(dto);
            if (!result.Succeeded)
            {
                return StatusCode((int)result.StatusCode, result);
            }
            return Ok(result);
        }
        [HttpPost(Router.AuthenticationRouting.Login)]
        public async Task<IActionResult> Login([FromForm] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authServices.LoginAsync(dto);
            if (!result.Succeeded)
            {
                return StatusCode((int)result.StatusCode, result);
            }
            return Ok(result);
        }
    }
}