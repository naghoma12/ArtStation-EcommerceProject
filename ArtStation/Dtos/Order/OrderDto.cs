namespace ArtStation.Dtos.Order
{
    public class OrderDto
    {
        public string CartId { get; set; }
       
        public int AddressId { get; set; }
        public string PaymentType { get; set; }
    }
}
