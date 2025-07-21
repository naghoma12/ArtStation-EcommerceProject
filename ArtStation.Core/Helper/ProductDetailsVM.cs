using ArtStation.Core.Entities;
using ArtStation.Core.Helper;

namespace ArtStation_Dashboard.ViewModels
{
    public class ProductDetailsVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShippingDetails { get; set; }

        public string DeliveredOn { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string Trader { get; set; }

        public List<string> Images { get; set; } = new List<string>();
        public List<string> Colors { get; set; } = new List<string>();
        public List<SizesDTO> Sizes { get; set; } = new List<SizesDTO>();
        public List<string> Flavours { get; set; } = new List<string>();
        public List<string> ForWhoms { get; set; } = new List<string>();
    }
}
