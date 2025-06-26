
using ArtStation.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace ArtStation.Dtos.AuthDtos
{
    public class LoginDto
    {
        [Required(ErrorMessageResourceType = typeof(Messages),
             ErrorMessageResourceName = "RequiredField")]
        [RegularExpression(@"^\+20\d{10}$",
            ErrorMessageResourceType = typeof(Messages),
            ErrorMessageResourceName = "InvalidPhoneNumber")]
        public string PhoneNumber { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Messages),
        //     ErrorMessageResourceName = "RequiredField")]
        public string? Code { get; set; }
    }
}
