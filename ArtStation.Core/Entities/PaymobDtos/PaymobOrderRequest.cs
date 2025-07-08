namespace ArtStation.Core.Entities.PaymobDtos
{
    public class PaymobOrderRequest
    {
        public string auth_token { get; set; }
        public Order.Order order { get; set; }
    }
}
