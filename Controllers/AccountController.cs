using Microsoft.AspNetCore.Mvc;
using Together.DTOs.App;
using Together.DTOs.User;
using Together.Models;
using Together.Services;

namespace Together.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;
        public AccountController(AccountService accountService)
        {
            _accountService = accountService; 
        }

        [HttpGet]
        public async Task<ActionResult<List<ViewUserDto>>> GetAllUsers()
        {
            var result = await _accountService.GetAllAccounts();
            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ViewUserDto>> GetUserById(int id)
        {
            var result = await _accountService.GetAccountById(id);
            if (result == null)
            {
                return NotFound("User not found.");
            }
            return result;
        }

        [HttpPost("sign-up")]
        public async Task<ActionResult> CreateUser([FromForm] CreateUserDto dto)
        {
            var result = await _accountService.CreateAccount(dto, Models.AccountRole.Volunteer, dto.ImageUrl);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _accountService.LoginAsync(dto);

            if (!result.Success)
                return Unauthorized(new { Message = result.Message });

            return Ok(new
            {
                result.Token, result.Role, result.Message
            });
        }   

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromForm] UpdateUserDto dto)
        {
            var result = await _accountService.UpdateAccount(id, dto, dto.ProfileImageUrl);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var result = await  _accountService.DeleteAccount(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }

        [HttpPut("change-password/{id}")]
        public async Task<ActionResult> ChangePassword(int id, ChangePasswordDto dto)
        {
            var result = await _accountService.ChangePassword(id, dto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<ViewUserDto>>> GetUsersByFilter(
            [FromQuery] string? name = null,
            [FromQuery] string? email = null,
            [FromQuery] AccountRole? role = null,
            [FromQuery] AccountStatus? status = null)
        {
            var filter = new UserFilterDto
            {
                Role = role,
                Status = status,
                Name = name,
                Email = email
            };

            var result = await _accountService.GetAccountsByFilter(filter);
            return result;
        }
    }
}