using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities
{
    public class ProductColor : BaseEntity
    {
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        [MaxLength(30)]
        public string  NameAR { get; set; }
        [MaxLength(30)]
        public string NameEN { get; set; }
        public string HexCode { get; set; }
    }
}
