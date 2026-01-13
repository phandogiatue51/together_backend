using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Together.Services;
using static Together.DTOs.Record.AttendanceDto;

namespace Together.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceService _attendanceService;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(AttendanceService attendanceService, ILogger<AttendanceController> logger)
        {
            _attendanceService = attendanceService;
            _logger = logger;
        }

        [HttpPost("generate-code")]
        public async Task<IActionResult> GenerateCode([FromBody] GenerateCodeRequestDto request)
        {
            try
            {
                var result = await _attendanceService.GenerateAttendanceCodeAsync(
                    request.ProjectId,
                    request.ActionType
                );

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating attendance code");
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequestDto request)
        {
            try
            {
                var result = await _attendanceService.VerifyAttendanceCodeAsync(request.Code, request.AccountId);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying attendance code");
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}