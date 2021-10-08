using System.Text.Json.Serialization;

namespace Server
{
    public class UserAuthentification
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }

        [JsonPropertyName("passwordHash")]
        public byte[] PasswordHash { get; set; }
    }
}
