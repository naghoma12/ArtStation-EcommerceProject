using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities;
using ArtStation.Resources;
using System.ComponentModel.DataAnnotations;

namespace ArtStation.Dtos.UserDtos
{
    public class AddressDto
    {
        [Required(ErrorMessageResourceType = typeof(Messages),
     ErrorMessageResourceName = "RequiredField")]
        public string FullName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages),
       ErrorMessageResourceName = "RequiredField")]
        [RegularExpression(@"^\+20\d{10}$",
     ErrorMessageResourceType = typeof(Messages),
     ErrorMessageResourceName = "InvalidPhoneNumber")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages),
     ErrorMessageResourceName = "RequiredField")]
        public int ShippingId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Messages),
     ErrorMessageResourceName = "RequiredField")]
        public string AddressDetails { get; set; }


        public decimal? Lat { get; set; }
        public decimal? Long { get; set; }
       
       
    }
}
