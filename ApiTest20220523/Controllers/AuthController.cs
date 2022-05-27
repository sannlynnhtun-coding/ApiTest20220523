using ApiTest20220523.Models;
using ApiTest20220523.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTest20220523.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtAuth _jwtAuth;

        public AuthController(IJwtAuth jwtAuth)
        {
            _jwtAuth = jwtAuth;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Authentication([FromBody] UserCredentialModel userCredential)
        {
            var token = _jwtAuth.Authentication(userCredential.UserName, userCredential.Password);
            if (token == null)
                return Unauthorized();
            return Ok(new { token = token });
        }
    }
}
