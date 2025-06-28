using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities
{
    public class ForWhom : BaseEntity
    {
        [MaxLength(10)]
        public string NameAR { get; set; }
        [MaxLength(10)]
        public string NameEN { get; set; }
        public ICollection<Product> products { get; set; } = new List<Product>();

    }
}
