using System.Text.Json.Serialization;

namespace GigachatServer.Models
{
    public class UserProfile
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("surname")]
        public string Surname { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("profileImage")]
        public byte[] ProfileImage { get; set; }
    }
}