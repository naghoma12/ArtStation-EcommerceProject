using ArtStation.Resources;
using System.ComponentModel.DataAnnotations;

namespace ArtStation.Dtos.UserDtos
{
    public class UserProfilePhotoDto
    {
        [Required(ErrorMessageResourceType = typeof(Messages),
             ErrorMessageResourceName = "RequiredField")]
        public IFormFile Photo { get; set; }
    }
}
