using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecoveryAPI.Models;

namespace RecoveryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet("token")]
        public object GetToken()
        {
            return AuthOptions.GenerateToken();
        }

        [HttpGet("token/secret")]
        public object GetAdminToken()
        {
            return AuthOptions.GenerateToken(true);
        }
    }
}
