using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Together.Services;
using static Together.DTOs.Record.QrDto;

namespace Together.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRController : ControllerBase
    {
        private readonly QrService _qrService;
        private readonly ILogger<QRController> _logger;

        public QRController(QrService qrService, ILogger<QRController> logger)
        {
            _qrService = qrService;
            _logger = logger;
        }

        [HttpPost("generate-checkin")]
        public async Task<IActionResult> GenerateCheckInQr([FromQuery] int projectId) 
        {
            try
            {
                var result = await _qrService.GenerateCheckInQrCodeAsync(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating check-in QR code");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("generate-checkout")]
        public async Task<IActionResult> GenerateCheckOutQr([FromQuery] int projectId) 
        {
            try
            {
                var result = await _qrService.GenerateCheckOutQrCodeAsync(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating check-out QR code");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("scan")]
        public async Task<IActionResult> ScanQr([FromForm] QrActionDto dto)
        {
            try
            {
                var result = await _qrService.ProcessQrScanAsync(dto);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing QR scan");
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}