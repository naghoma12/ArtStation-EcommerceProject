using ArtStation_Dashboard.Resource;
using System.ComponentModel.DataAnnotations;

namespace ArtStation_Dashboard.ViewModels
{
    public class SizeVM
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "RequiredField")]
        public string SizeAR { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "RequiredField")]
        public string SizeEN { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages), ErrorMessageResourceName = "RequiredField")]
        public decimal Price { get; set; }
    }
}
