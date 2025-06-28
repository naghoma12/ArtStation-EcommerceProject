using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ArtStation.Core.Resources;

namespace ArtStation.DTOS
{
    public class UserReview
    {

        [MaxLength(500 ,ErrorMessageResourceType = typeof(Messages)
            ,ErrorMessageResourceName = "CommentMaxLength")]
        public string? Comment { get; set; }

        [Range(1, 5 , ErrorMessageResourceType =(typeof(Messages)), 
            ErrorMessageResourceName = "Rating")]
        public float? Rating { get; set; }
        [Required(ErrorMessageResourceType = typeof(Messages), 
            ErrorMessageResourceName = "RequiredField")]
        public int ProductId { get; set; }
    }
}
