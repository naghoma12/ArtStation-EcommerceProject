using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ArtStation.Core.Helper.PaymobDtos
{
    public class CreateOrderResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
