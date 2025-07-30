using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Helper
{
    public class CategoryWithProducts
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public IEnumerable<SimpleProduct> Products { get; set; } = new List<SimpleProduct>();
    }

}
