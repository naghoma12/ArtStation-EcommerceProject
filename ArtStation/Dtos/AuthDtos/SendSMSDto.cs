using ArtStation.Resources;
using System.ComponentModel.DataAnnotations;

namespace ArtStation.Dtos.AuthDtos
{

    public class SendSMSDto
    {
        [Required(ErrorMessageResourceType = typeof(Messages),
          ErrorMessageResourceName = "RequiredField")]
        [RegularExpression(@"^\+20\d{10}$",
        ErrorMessageResourceType = typeof(Messages),
        ErrorMessageResourceName = "InvalidPhoneNumber")]
        public string PhoneNumber { get; set; }

    }
}

