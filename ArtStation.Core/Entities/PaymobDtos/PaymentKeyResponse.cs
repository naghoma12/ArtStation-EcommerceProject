using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.PaymobDtos
{
    public class PaymentKeyResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
