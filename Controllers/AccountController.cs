using Microsoft.AspNetCore.Mvc;
using Together.DTOs.User;
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

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _accountService.GetAllAccounts();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _accountService.GetAccountById(id);
            if (result == null)
            {
                return NotFound("User not found.");
            }
            return Ok(result);
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> CreateUser(CreateUserDto dto)
        {
            var result = await _accountService.CreateAccount(dto, Models.AccountRole.User);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _accountService.LoginAsync(dto);

            if (!result.Success)
                return Unauthorized(new { Message = result.Message });

            return Ok(new
            {
                Token = result.Token,
                Role = result.Role,
                Message = result.Message
            });
        }   

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto dto)
        {
            var result = await _accountService.UpdateAccount(id, dto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await  _accountService.DeleteAccount(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }

        [HttpPut("change-password/{id}")]
        public async Task<IActionResult> ChangePassword(int id, ChangePasswordDto dto)
        {
            var result = await _accountService.ChangePassword(id, dto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }
    }
}