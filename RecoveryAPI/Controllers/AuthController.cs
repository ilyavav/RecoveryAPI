using Microsoft.AspNetCore.Mvc;
using RecoveryAPI.Dtos;
using RecoveryAPI.Models;
using RecoveryAPI.Services;

namespace RecoveryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthManager _authManager;

        public AuthController(AuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost]
        public ActionResult<TokenDto> GetToken([FromBody] LoginData loginData)
        {
            var user = _authManager.Authenticate(loginData.Login, loginData.Password);

            if (user == null)
            {
                return Unauthorized(new ErrorDto { Message = "Wrong login or password." });
            }

            return Ok(AuthOptions.GenerateToken(user.Login, user.IsAdmin));
        }
    }
}