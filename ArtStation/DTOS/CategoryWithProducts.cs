using ArtStation.Core.Entities;

namespace ArtStation.DTOS
{
    public class CategoryWithProducts
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<SimpleProduct> Products { get; set; } = new List<SimpleProduct>();
    }
}
