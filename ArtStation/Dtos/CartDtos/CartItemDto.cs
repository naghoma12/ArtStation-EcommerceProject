namespace ArtStation.Dtos.CartDtos
{
    public class CartItemDto
    {
        public int ProductId { get; set; }

        public int? ColorId { get; set; }
        public int? SizeId { get; set; }
        public int? FlavourId { get; set; }
        public int Quantity { get; set; }
    }
}
