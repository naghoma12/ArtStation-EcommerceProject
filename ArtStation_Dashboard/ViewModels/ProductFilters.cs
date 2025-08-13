using ArtStation.Core.Helper;

namespace ArtStation_Dashboard.ViewModels
{
    public class ProductFilters
    {
        public IEnumerable<SimpleCategoryDTO> Categories { get; set; }
        public IEnumerable<BrandDTO> Brands { get; set; }
    }
}
