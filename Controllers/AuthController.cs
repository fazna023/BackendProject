using BackendProject2.Dto;
using BackendProject2.Services.AuthServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BackendProject2.ApiResponse;

namespace BackendProject2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto newUser)
        {
            try
            {
                bool isdone = await _authServices.Register(newUser);
                if (!isdone)
                {
                    return BadRequest(new ApiResponse<string>(false, "User already exists", "[]", null));
                }

                return Ok(new ApiResponse<string>(true, "User registerd succesfully", "[done]", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "server Error");
            }
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto login)
        {
            try
            {
                var res = await _authServices.Login(login);

                if (res.Error == "Not Found")
                {
                    return NotFound("Email is not verified");
                }

                if (res.Error == "invalid password")
                {
                    return BadRequest(res.Error);
                }

                if (res.Error == "User Blocked")
                {
                    return StatusCode(403, "User is blocked by admin!");
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
