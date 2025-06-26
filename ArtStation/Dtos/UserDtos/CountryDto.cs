
using ArtStation.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace ArtStation.Dtos.UserDtos
{
    public class CountryDto
    {
        [Required(ErrorMessageResourceType = typeof(Messages),
    ErrorMessageResourceName = "RequiredField")]
        public string Country { get; set; }
    }
}
