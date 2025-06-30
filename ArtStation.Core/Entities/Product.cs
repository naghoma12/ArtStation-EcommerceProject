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
        [MaxLength(30)]
        public string NameAR { get; set; }
        [MaxLength(30)]
        public string NameEN { get; set; }
        [MaxLength(1000)]
        public string DescriptionAR { get; set; }
        [MaxLength(1000)]
        public string DescriptionEN { get; set; }
        [MaxLength(150)]
        public string ShippingDetailsAR { get; set; }
        [MaxLength(150)]
        public string ShippingDetailsEN { get; set; }
        [MaxLength(150)]
        public string DeliveredOnAR { get; set; }
        [MaxLength(150)]
        public string DeliveredOnEN { get; set; }
        [MaxLength(50)]
        public string BrandAR { get; set; }
        [MaxLength(50)]
        public string BrandEN { get; set; }
        public int SellersCount { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }

        public ICollection<ProductPhotos> ProductPhotos { get; set; } = new List<ProductPhotos>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<ProductColor> ProductColors { get; set; } = new List<ProductColor>();
        public ICollection<ProductFlavour> ProductFlavours { get; set; } = new List<ProductFlavour>();
        public ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
        public  ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
   
    public enum ForWhom
    {
        Women = 0,
        نساء = 0,
        Kids =1 ,
        أطفال = 1,
        Men =2,
        رجال = 2,
    }
}
