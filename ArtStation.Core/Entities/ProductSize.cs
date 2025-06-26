using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities
{
    public class ProductSize : BaseEntity
    {
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        [MaxLength(30)]
        public string SizeAR { get; set; }
        [MaxLength(30)]
        public string SizeEN { get; set; }
        public decimal Price { get; set; }

    }
}
