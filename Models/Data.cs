using System.Text.Json.Serialization;

namespace GigachatServer.Models
{
    public class Data
    {
        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; }

        [JsonPropertyName("userAuthentification")]
        public UserAuthentification UserAuthentification { get; set; }

        [JsonPropertyName("userRegistration")]
        public UserRegistration UserRegistration { get; set; }

        [JsonPropertyName("userProfile")]
        public UserProfile UserProfile { get; set; }

        [JsonPropertyName("response")]
        public Response Response { get; set; }

        [JsonPropertyName("profileImage")]
        public ProfileImage ProfileImage { get; set; }
    }
}
