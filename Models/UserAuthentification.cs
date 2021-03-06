using System.Text.Json.Serialization;

namespace GigachatServer.Models
{
    public class UserAuthentification
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
