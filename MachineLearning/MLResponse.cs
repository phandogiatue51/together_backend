using System.Text.Json.Serialization;

namespace Together.MachineLearning
{
    public class MLResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("intent")]
        public string Intent { get; set; }

        [JsonPropertyName("reply")]
        public string Reply { get; set; }

        [JsonPropertyName("api_endpoint")]
        public string ApiEndpoint { get; set; }

        [JsonPropertyName("needs_api")]
        public bool NeedsApi { get; set; }

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }
    }
}
