


namespace ArtStation.Dtos.PaymobDtos
{
    public class PaymobOrderRequest
    {
        public string auth_token { get; set; }
        public ArtStation.Core.Entities.Order.Order order { get; set; }
    }
}
