using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Together.DTOs.App;
using Together.DTOs.Certi;
using Together.DTOs.Organ;
using Together.Models;
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
        public async Task<ActionResult<List<ViewCertiDto>>> GetAllCertis()
        {
            var certis = await _certificateService.GetAllCertificatesAsync();
            return certis;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ViewCertiDto>> GetCertiById(int id)
        {
            var certis = await _certificateService.GetCertificateByIdAsync(id);
            if (certis == null)
                return NotFound();
            return certis;
        }

        [HttpGet("account/{accountId}")]
        public async Task<ActionResult> GetCertisByAccountId(int accountId)
        {
            var certis = await _certificateService.GetCertificatesByAccountIdAsync(accountId);
            if (certis == null)
                return NotFound();
            return Ok(certis);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCerti([FromForm] CreateCertiDto dto)
        {
            var result = await _certificateService.CreateCertificateAsync(dto, dto.ImageUrl);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCerti(int id, [FromForm] CreateCertiDto dto)
        {
            var result = await _certificateService.UpdateCertificateAsync(id, dto, dto.ImageUrl);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCerti(int id)
        {
            var result = await _certificateService.DeleteCertificateAsync(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<ViewCertiDto>>> FilterCertis(
            [FromQuery] int? accountId = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] string? certificateName = null)
        {
            var filter = new CertiFilterDto
            {
                AccountId = accountId,
                CategoryId = categoryId,
                CertificateName = certificateName,
            };
          
            var certis = await _certificateService.FilterCertificatesAsync(filter);
            return certis;
        }
    }
}
