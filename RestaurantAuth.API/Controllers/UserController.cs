using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantAuth.Infrastructure;
using RestaurantAuth.Domain;
using RestaurantAuth.Domain.DTO.User;

namespace RestaurantAuth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository= userRepository;
        }
        [Obsolete]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();
            return Ok(users);
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterationRequestDTO request)
        {
            try
            {
                var user = await _userRepository.RegisterUser(request);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "User registration failed.",
                        data = (object?)null
                    });
                }

                return StatusCode(StatusCodes.Status201Created, new
                {
                    success = true,
                    message = "User registered successfully.",
                    data = user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An unexpected error occurred.",
                    error = ex.Message
                });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequestDTO request)
        {
            try
            {
                var response = await _userRepository.LoginUser(request);

                return Ok(new
                {
                    success = true,
                    message = "Login successful.",
                    data = response
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = "An unexpected error occurred.",
                    error = ex.InnerException != null ? ex.InnerException.Message : ex.Message
                });
            }
        }
    }
}
