using System.Text.Json.Serialization;

namespace ArtStation.Core.Entities.PaymobDtos
{
    public class PaymobAuthResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
