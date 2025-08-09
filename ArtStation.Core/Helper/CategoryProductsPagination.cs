using ArtStation_Dashboard.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Helper
{
    public class CategoryProductsPagination
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        public PagedResult<SimpleProductVM> ProductsPaged { get; set; }
    }
}
