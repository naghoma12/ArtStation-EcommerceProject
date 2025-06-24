namespace ArtStation.Dtos.AuthDtos
{
    public class AddressDtoUseId
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }

        public int ShippingId { get; set; }
        public string AddressDetails { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Long { get; set; }
     
    }
}
