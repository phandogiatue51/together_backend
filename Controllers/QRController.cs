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

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateQr([FromBody] GenerateQrDto dto)
        {
            try
            {
                var result = await _qrService.GenerateQrCodeAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code");
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