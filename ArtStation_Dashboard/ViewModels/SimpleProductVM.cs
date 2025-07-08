namespace ArtStation_Dashboard.ViewModels
{
    public class SimpleProductVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAfterSale { get; set; }
        public string Image { get; set; }

        public string CategoryName { get; set; }
        public string UserName { get; set; } 
    }
}
