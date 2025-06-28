
using ArtStation.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace ArtStation.Dtos.UserDtos
{
    public class UpdateUserProfileDto
    {
        [Required(ErrorMessageResourceType = typeof(Messages),
             ErrorMessageResourceName = "RequiredField")]
        public string Fname { get; set; }
        [Required(ErrorMessageResourceType = typeof(Messages),
             ErrorMessageResourceName = "RequiredField")]
        public string LName { get; set; }

        public string? Email { get; set; }
        [Required(ErrorMessageResourceType = typeof(Messages),
              ErrorMessageResourceName = "RequiredField")]
        [RegularExpression(@"^\+20\d{10}$",
          ErrorMessageResourceType = typeof(Messages),
          ErrorMessageResourceName = "InvalidPhoneNumber")]
        public string PhoneNumber { get; set; }
       
        public string? Birthday { get; set; }
        public string? Nationality { get; set; }
        public string? Gender { get; set; }
       
    }
}
