using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.AI
{
    public class RecommendedProduct: BaseEntity
    {
        public Product Product { get; set; }
        public int ProductId { get; set; }

        public int ChatResponseRecommendationId { get; set; }
        public ChatResponseRecommendation ChatResponseRecommendation { get; set; }

    }
}
