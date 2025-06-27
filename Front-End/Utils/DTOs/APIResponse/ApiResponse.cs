using System.Text.Json.Serialization;

namespace Utils.DTOs.ApiResponse
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }
        public object? AdditionalData { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public string? Code { get; set; }
    }
}
