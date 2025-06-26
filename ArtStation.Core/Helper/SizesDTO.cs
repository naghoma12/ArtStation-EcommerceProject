using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Helper
{
    public class SizesDTO
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public decimal? PriceAfterSale { get; set; }
        public string Size { get; set; }
    }
}
