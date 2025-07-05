namespace ArtStation.Dtos.PaymobDtos
{
    public class PaymobCallbackDto
    {
        public PaymobCallbackObj obj { get; set; }
    }

    public class PaymobCallbackObj
    {
        public PaymobCallbackTransaction transaction { get; set; }
        public PaymobCallbackOrder order { get; set; }
    }

    public class PaymobCallbackTransaction
    {
        public int id { get; set; }
        public bool success { get; set; }
        public bool is_captured { get; set; }
        public int amount_cents { get; set; }
        public string currency { get; set; }
    }

    public class PaymobCallbackOrder
    {
        public int id { get; set; }
    }

}
