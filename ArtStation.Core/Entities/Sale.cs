using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities
{
    public class Sale:BaseEntity
    {
        public int Discount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Product Product { get; set; }
        public int ProductId { get; set; }

    }
}
