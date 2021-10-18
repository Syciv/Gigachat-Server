using System.Text.Json.Serialization;

namespace GigachatServer.Models
{
    public class User
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
    }
}
