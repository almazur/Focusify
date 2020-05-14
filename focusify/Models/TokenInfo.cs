using System.Text.Json;
using System.Text.Json.Serialization;

namespace focusify.Models
{
    public class TokenInfo
    {
        public static TokenInfo fromJson(string json)
        {
            return JsonSerializer.Deserialize<TokenInfo>(json);
        }
        [JsonPropertyName("access_token")]
        public string accessToken { get; set; }
        [JsonPropertyName("token_type")]
        public string tokenType { get; set; }

        [JsonPropertyName("scope")]
        public string scope { get; set; }

        [JsonPropertyName("expires_in")]
        public int expiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string refreshToken { get; set; }

    }
}