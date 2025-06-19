using ArtStation.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities
{
    public class Product: BaseEntity
    {
        [MaxLength(20)]
        public string Name { get; set; }
        public string Description { get; set; }
        public ForWhom ForWhom { get; set; }
        public decimal Price { get; set; }
        public string ShippingDetails { get; set; }
        public string Brand { get; set; }
        public int SellersCount { get; set; }
        public string Language { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public ICollection<ProductPhotos> ProductPhotos { get; set; } = new List<ProductPhotos>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<ProductFlavour> ProductFlavours { get; set; } = new List<ProductFlavour>();
        public ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
        public ICollection<Favourite> Favourites = new List<Favourite>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
    [Flags]
    public enum ForWhom
    {
        Women = 0,
        Men = 2,
        Kids = 4
    }
}
