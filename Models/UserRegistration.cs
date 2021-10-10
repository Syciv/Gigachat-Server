using System.Text.Json.Serialization;

namespace Server
{
    public class UserRegistration
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("surname")]
        public string Surname { get; set; }

        [JsonPropertyName("passwordHash")]
        public byte[] PasswordHash { get; set; }

        [JsonPropertyName("salt")]
        public byte[] Salt { get; set; }

    }
}
