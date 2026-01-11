using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Together.DTOs.Record;
using Together.Services;

namespace Together.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HourController : ControllerBase
    {
        private readonly HourService _hourService;
        public HourController(HourService hourService)
        {
            _hourService = hourService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ViewRecordDto>>> GetAllRecords()
        {
            var records = await _hourService.GetAllRecords();
            return records;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ViewRecordDto>> GetRecordById(int id)
        {
            var record = await _hourService.GetRecordByIdAsync(id);
            return record;
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<List<ViewRecordDto>>> GetRecordsByProjectId(int projectId)
        {
            var records = await _hourService.GetRecordByProjectId(projectId);
            return records;
        }
    }
}
