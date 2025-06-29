namespace ArtStation.Core.Helper
{
    public class CartItemReturnDto
    {
        public string ItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public string PhotoUrl { get; set; }

        public string? Color { get; set; }
        public string? Size { get; set; }

        public string? Flavour { get; set; }
        public decimal Price { get; set; }

        public decimal? PriceAfterSale { get; set; } = 0.0m;


        public int Quantity { get; set; }
    }
}
