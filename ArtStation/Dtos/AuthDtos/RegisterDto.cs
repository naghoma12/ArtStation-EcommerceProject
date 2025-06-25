using ArtStation.Resources;
using System.ComponentModel.DataAnnotations;

namespace ArtStation.Dtos.AuthDtos
{
    public class RegisterDto
    {
        [Required(ErrorMessageResourceType = typeof(Messages),
        ErrorMessageResourceName = "RequiredField")]
        [RegularExpression(@"^\+20\d{10}$",
      ErrorMessageResourceType = typeof(Messages),
      ErrorMessageResourceName = "InvalidPhoneNumber")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages),
       ErrorMessageResourceName = "RequiredField")]
        [RegularExpression(@"^\S+\s\S+.*$",
      ErrorMessageResourceType = typeof(Messages),
      ErrorMessageResourceName = "InvalidFullName")]
        public string FullName { get; set; }

       // [Required(ErrorMessageResourceType = typeof(Messages),
       //ErrorMessageResourceName = "RequiredField")]
        public string? Code { get; set; }
    }
}
