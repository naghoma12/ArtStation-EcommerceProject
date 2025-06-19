using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities
{
    public class ProductPhotos: BaseEntity
    {
            public string Photo { get; set; }

            public virtual Product Product { get; set; }

            public int ProductId { get; set; }

        
    }
}
