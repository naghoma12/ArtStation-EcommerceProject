namespace ArtStation.Dtos.CartDtos
{
    public class CartItemDto
    {
       
        public int ProductId { get; set; }
        public string? ColorId { get; set; }
        public string? SizeId { get; set; }
        public string? FlavourId { get; set; }

        public int Quantity { get; set; }
    }
}
