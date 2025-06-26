using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities
{
    public class Category : BaseEntity
    {
        public string NameAR { get; set; }
        public string NameEN { get; set; }
        public string Image { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
