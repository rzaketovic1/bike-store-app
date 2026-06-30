using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController(IUserService userService, ITokenService tokenService) : ControllerBase
    {
        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="registerDto">User registration details</param>
        /// <response code="201">User successfully registered with authentication token</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="409">Email already in use</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserDto>> Register(UserRegisterDto registerDto)
        {
            var user = await userService.CreateAsync(registerDto.Email, registerDto.Password, registerDto.DisplayName);
            var token = tokenService.CreateToken(user);

            return StatusCode(StatusCodes.Status201Created, new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = token
            });
        }

        /// <summary>
        /// Authenticate user and get access token
        /// </summary>
        /// <param name="loginDto">User login credentials</param>
        /// <response code="200">Login successful, returns user details with JWT token</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Invalid email or password</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> Login(UserLoginDto loginDto)
        {
            var user = await userService.AuthenticateAsync(loginDto.Email, loginDto.Password);
            var token = tokenService.CreateToken(user);

            return Ok(new UserDto
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = token
            });
        }
    }
}
