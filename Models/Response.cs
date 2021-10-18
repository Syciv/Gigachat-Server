using System.Text.Json.Serialization;

namespace GigachatServer.Models
{
    public class Response
    {
        [JsonPropertyName("result")]
        public int Result { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; }
    }
}
