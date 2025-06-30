using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities
{
    public class ProductForWhom : BaseEntity
    {
        public string ForWhomAR { get; set; }
        public string ForWhomEN { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }

    }
}
