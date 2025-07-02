using ArtStation.Core.Resources;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ArtStation_Dashboard.ViewModels
{
    public class CreatedCategory
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        public string NameAR { get; set; }
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        public string NameEN { get; set; }
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "RequiredField")]
        public IFormFile PhotoFile { get; set; }
        public string? PhotoURL { get; set; }
    }
}
