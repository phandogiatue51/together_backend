using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Together.DTOs.Certi;
using Together.DTOs.Organ;
using Together.Services;

namespace Together.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        private readonly CertificateService _certificateService;
        public CertificateController(CertificateService certificateService)
        {
            _certificateService = certificateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCertis()
        {
            var certis = await _certificateService.GetAllCertificatesAsync();
            return Ok(certis);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCertiById(int id)
        {
            var certis = await _certificateService.GetCertificateByIdAsync(id);
            if (certis == null)
                return NotFound();
            return Ok(certis);
        }

        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetCertisByAccountId(int accountId)
        {
            var certis = await _certificateService.GetCertificatesByAccountIdAsync(accountId);
            if (certis == null)
                return NotFound();
            return Ok(certis);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCerti([FromForm] CreateCertiDto dto)
        {
            var result = await _certificateService.CreateCertificateAsync(dto, dto.ImageUrl);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCerti(int id, [FromForm] CreateCertiDto dto)
        {
            var result = await _certificateService.UpdateCertificateAsync(id, dto, dto.ImageUrl);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCerti(int id)
        {
            var result = await _certificateService.DeleteCertificateAsync(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }
    }
}
