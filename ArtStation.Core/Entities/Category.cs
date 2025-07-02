using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities
{
    public class Category : BaseEntity
    {
        [DisplayName("اسم التصنيف")]
        public string NameAR { get; set; }
        [DisplayName("Cantegory Name")]
        public string NameEN { get; set; }
        public string Image { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
