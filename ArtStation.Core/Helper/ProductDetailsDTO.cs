using ArtStation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Helper
{
    public class ProductDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public float AvgRating { get; set; }
        public int ReviewsNumber { get; set; }

        public List<ColorsDTO> Colors { get; set; } = new List<ColorsDTO>();
        public List<SizesDTO> Sizes { get; set; } = new List<SizesDTO>();
        public List<string> Flavours { get; set; } = new List<string>();
        public string ShippingDetails { get; set; }
        public int DeliveredMinDate { get; set; }
        public int DeliveredMaxDate { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();


    }
}
