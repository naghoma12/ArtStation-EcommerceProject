using ArtStation.Core.Entities;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Helper;
using ArtStation_Dashboard.Resource;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ArtStation_Dashboard.ViewModels
{
    public class ProductCreation
    {
        public int Id { get; set; }
        [MaxLength(30, ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "MaxLength30")]
        public string? NameAR { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "RequiredField")]
        [MaxLength(30, ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "MaxLength30")]
        public string NameEN { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "RequiredField")]
        [MaxLength(1000, ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "MaxLength1000")]
        public string DescriptionAR { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "RequiredField")]
        [MaxLength(1000, ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "MaxLength1000")]
        public string DescriptionEN { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "RequiredField")]
        [MaxLength(50, ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "MaxLength50")]
        public string BrandAR { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "RequiredField")]
        [MaxLength(50, ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "MaxLength50")]
        public string BrandEN { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "RequiredField")]
        [MaxLength(150, ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "MaxLength150")]
        public string ShippingDetailsAR { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "RequiredField")]
        [MaxLength(150, ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "MaxLength150")]
        public string ShippingDetailsEN { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "RequiredField")]
        [MaxLength(150, ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "MaxLength150")]
        public string DeliveredOnAR { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "RequiredField")]
        [MaxLength(150, ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "MaxLength150")]
        public string DeliveredOnEN { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "RequiredField")]
        public int CategoryId { get; set; }
        public int? TraderId { get; set; }
        public string? TraderName { get; set; }
        public int ForWhomId { get; set; }
        public int SellersCount { get; set; }
        public SaleVM Sale { get; set; } = new SaleVM();
        public IEnumerable<SimpleCategoryDTO> Categories { get; set; } = new List<SimpleCategoryDTO>();
        public IEnumerable<AppUser> Traders { get; set; } = new List<AppUser>();
        public List<string> Images { get; set; } = new List<string>();
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();

        public List<ColorVM> Colors { get; set; } = new List<ColorVM>();
        public List<SizeVM> Sizes { get; set; } = new List<SizeVM>();
        public List<FlavourVM> Flavours { get; set; } = new List<FlavourVM>();
        public List<ForWhomWithId> forWhoms { get; set; } = new List<ForWhomWithId>();
        public List<string> SelectedForWhoms { get; set; } = new List<string>();
    }
}
