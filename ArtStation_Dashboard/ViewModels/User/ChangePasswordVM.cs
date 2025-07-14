using ArtStation_Dashboard.Resource;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace ArtStation_Dashboard.ViewModels.User
{
    public class ChangePasswordVM
    {

        public int TraderId { get; set; }
        [Required(ErrorMessageResourceType = typeof(AnnotationMessages),
            ErrorMessageResourceName = "RequiredField")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{5,}$",
    ErrorMessageResourceType = typeof(AnnotationMessages),
    ErrorMessageResourceName = "PasswordComplexity")]

        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(AnnotationMessages),
            ErrorMessageResourceName = "RequiredField")]
        [Compare("NewPassword",
    ErrorMessageResourceType = typeof(AnnotationMessages),
    ErrorMessageResourceName = "ConfirmPasswordMismatch")]
        public string ConfirmPassword { get; set; }
    }
}
