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

        public List<ProductPhotos> ProductPhotos { get; set; } = new List<ProductPhotos>();
        public List<Sale> Sales { get; set; } = new List<Sale>();
        public List<ProductColor> ProductColors { get; set; } = new List<ProductColor>();
        public List<ProductFlavour> ProductFlavours { get; set; } = new List<ProductFlavour>();
        public List<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
        public  ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public List<ProductForWhom> ForWhoms { get; set; } = new List<ProductForWhom>();
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        public static implicit operator string(Product v)
        {
            throw new NotImplementedException();
        }
    }
   
    public enum ForWhom
    {
        Women = 1,
        Men =2,
        Kids =3
    }
}
