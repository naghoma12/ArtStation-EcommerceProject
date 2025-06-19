using ArtStation.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities
{
    public class Favourite : BaseEntity
    {
        [ForeignKey(nameof(AppUser))]
        public int UserId { get; set; }
        public AppUser User { get; set; }
        [ForeignKey(nameof(Product))]
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
