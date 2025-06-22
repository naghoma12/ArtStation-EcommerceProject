using ArtStation.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;

namespace ArtStation.Core.Entities
{
    public class Review: BaseEntity
    {
        public string? Comment { get; set; }

        [Range(1, 5)]
        public float? Rating { get; set; }

        public virtual Product Product { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public AppUser? AppUser { get; set; }
        [ForeignKey(nameof(AppUser))]
        public int UserId { get; set; }
        public int LikesCount { get; set; }

    }
}
