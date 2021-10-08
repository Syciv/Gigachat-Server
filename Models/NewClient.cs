using System.Text.Json.Serialization;

namespace Server
{
    public class NewClient
    {
        [JsonPropertyName("clientName")]
        public string ClientName { get; set; }
    }
}
