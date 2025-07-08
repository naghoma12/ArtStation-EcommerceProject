using ArtStation.Core.Resources;
using ArtStation_Dashboard.Resource;
using System.ComponentModel.DataAnnotations;

namespace ArtStation_Dashboard.ViewModels.User
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages),
            ErrorMessageResourceName = "RequiredField")]
        public string UserName { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages),
        ErrorMessageResourceName = "RequiredField")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
