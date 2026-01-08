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
        public async Task<IActionResult> GenerateCheckInQr([FromBody] GenerateQrDto dto)
        {
            try
            {
                var result = await _qrService.GenerateCheckInQrCodeAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating check-in QR code");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("generate-checkout")]
        public async Task<IActionResult> GenerateCheckOutQr([FromBody] GenerateQrDto dto)
        {
            try
            {
                var result = await _qrService.GenerateCheckOutQrCodeAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating check-out QR code");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("scan")]
        public async Task<IActionResult> ScanQr([FromBody] QrActionDto dto)
        {
            try
            {
                var volunteerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var result = await _qrService.ProcessQrScanAsync(dto, volunteerId);

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