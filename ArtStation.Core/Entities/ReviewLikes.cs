using ArtStation.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities
{
    public class ReviewLikes : BaseEntity
    {
        [ForeignKey(nameof(AppUser))]
        public int UserId { get; set; }
        public AppUser User { get; set; }
        [ForeignKey(nameof(Review))]
        public int? ReviewId { get; set; }
        public Review? Review { get; set; }
    }
}
