using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities
{
    public class Banner:BaseEntity
    {
        public string Title { get; set; }
       
        public string ImageUrl { get; set; }

        public int Order { get; set; }
    }
}
