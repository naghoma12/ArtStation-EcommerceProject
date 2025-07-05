using System.Text.Json.Serialization;

namespace ArtStation.Dtos.PaymobDtos
{
    public class PaymobAuthResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
