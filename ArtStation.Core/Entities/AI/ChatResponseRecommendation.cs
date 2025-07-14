using ArtStation.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.AI
{
    public class ChatResponseRecommendation : BaseEntity
    {
        public string? Message { get; set; }
        public string? Image { get; set; }
        public string? Note { get; set; }
        public string Reply { get; set; }
        public int UserId { get; set; }

        public AppUser User { get; set; }
        public ICollection<RecommendedProduct> RecommendedProducts { get; set; } = new List<RecommendedProduct>();
    }
}
