using System.ComponentModel.DataAnnotations;

namespace ArtStation.DTOS
{
    public class SimpleProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ReviewsNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PriceAfterSale { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsActive { get; set; }
        public float? AvgRating { get; set; }
        public bool IsFav { get; set; } = false;
    }
}
