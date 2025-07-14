using ArtStation.Core.Entities;
using ArtStation.Core.Entities.Identity;
using ArtStation_Dashboard.Resource;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ArtStation_Dashboard.ViewModels.User
{
    public class TraderViewModel
    {
        public int? Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages),
        ErrorMessageResourceName = "RequiredField")]
        public string UserName { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages),
        ErrorMessageResourceName = "RequiredField")]
        [MaxLength(ErrorMessageResourceType = typeof(AnnotationMessages),
        ErrorMessageResourceName = "MaxName")]
        public string DispalyName { get; set; }
        public string? Email { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages),
        ErrorMessageResourceName = "RequiredField")]
        public string PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Nationality { get; set; }
        public string? Photo { get; set; }
        public IFormFile? PhotoFile { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages),
        ErrorMessageResourceName = "RequiredField")]
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<Shipping>   Cities { get; set; } = new List<Shipping>();
    }
}
