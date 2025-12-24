using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Together.MachineLearning;

namespace Together.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ChatbotController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskBot([FromBody] ChatRequest request)
        {
            var mlResponse = await _httpClient.PostAsJsonAsync(
                "http://localhost:8000/chat",
                new { message = request.Message }
            );

            var mlResult = await mlResponse.Content.ReadFromJsonAsync<MLResponse>();

            if (!mlResult.NeedsApi || string.IsNullOrEmpty(mlResult.ApiEndpoint))
            {
                return Ok(new { reply = mlResult.Reply, intent = mlResult.Intent });
            }

            var apiUrl = $"https://localhost:7085{mlResult.ApiEndpoint}";
            var dataResponse = await _httpClient.GetAsync(apiUrl);
            dataResponse.EnsureSuccessStatusCode();

            var jsonData = await dataResponse.Content.ReadAsStringAsync();
            var jArray = JArray.Parse(jsonData);
            var itemCount = jArray.Count;

            var summary = CreateGenericSummary(mlResult.ApiEndpoint, jArray);

            return Ok(new
            {
                reply = summary,
                intent = mlResult.Intent,
                data = jsonData,  
                raw_count = itemCount
            });
        }

        private string CreateGenericSummary(string apiEndpoint, JArray data)
        {
            var entityName = GetEntityNameFromEndpoint(apiEndpoint);
            var count = data.Count;

            if (count == 0) return $"Không tìm thấy {entityName} nào.";

            var summary = $"Tìm thấy {count} {entityName}:\n";

            foreach (var item in data.Take(5)) 
            {
                var name = item["name"] ?? item["Name"] ?? item["title"] ?? item["Title"] ?? "Không có tên";
                summary += $"- {name}\n";
            }

            if (count > 5) summary += $"... và {count - 5} {entityName} khác";

            return summary;
        }

        private string GetEntityNameFromEndpoint(string endpoint)
        {
            return endpoint.ToLower() switch
            {
                var x when x.Contains("organization") => "tổ chức",
                var x when x.Contains("account") || x.Contains("user") => "người dùng",
                _ => "mục"
            };
        }
    }
}