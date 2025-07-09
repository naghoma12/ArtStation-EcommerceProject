using ArtStation.Core.Resources;
using ArtStation_Dashboard.Resource;
using System.ComponentModel.DataAnnotations;

namespace ArtStation_Dashboard.ViewModels
{
    public class CategoryVM
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages),ErrorMessageResourceName = "RequiredField")]
        public string NameAR { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "RequiredField")]
        public string NameEN { get; set; }
        public IFormFile? PhotoFile { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Image { get; set; }
        public string? FilePath { get; set; }

    }
}
