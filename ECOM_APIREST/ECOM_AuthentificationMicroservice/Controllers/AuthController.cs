using ECOM_AuthentificationMicroservice.Models;
using ECOM_AuthentificationMicroservice.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ECOM_AuthentificationMicroservice.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var result = await _authService.AuthenticateAsync(loginModel.Email, loginModel.Password);
            
            if (!result.success)
            {
                return Unauthorized(new { message = "Email ou mot de passe incorrect" });
            }

            return Ok(new { token = $"Bearer {result.token}", user = result.user });
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                Email = registerModel.Email,
                Password = registerModel.Password,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                UserType = registerModel.UserType,
                CompanyName = registerModel.CompanyName,
                CompanyAddress = registerModel.CompanyAddress,
                CompanyPhone = registerModel.CompanyPhone,
                CompanyDescription = registerModel.CompanyDescription
            };

            var result = await _authService.RegisterAsync(user);

            if (!result.success)
            {
                return BadRequest(new { message = result.message });
            }

            return CreatedAtAction(nameof(Register), new { email = user.Email }, new { message = "User registered successfully" });
        }

        [HttpGet("validate")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ValidateToken([FromHeader(Name = "Authorization")] string authorization)
        {
            if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
            {
                return Unauthorized(new { message = "Invalid token format" });
            }

            var token = authorization.Substring("Bearer ".Length).Trim();
            
            if (!_authService.ValidateToken(token))
            {
                return Unauthorized(new { message = "Invalid or expired token" });
            }

            var user = _authService.GetUserFromToken(token);
            return Ok(new { isValid = true, user });
        }
    }
} 