using System.Text.Json.Serialization;

namespace GigachatServer.Models
{
    public class ProfileImage
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("image")]
        public byte[] Image { get; set; }
    }
}
