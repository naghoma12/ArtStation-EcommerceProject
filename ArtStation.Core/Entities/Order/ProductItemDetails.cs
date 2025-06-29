using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Order
{
    public class ProductItemDetails
    {
        public ProductItemDetails()
        {

        }

        public ProductItemDetails(int productId, int? colorId, int? sizeId, int? flavourId)
        {
            ProductId = productId;
           
            ColorId= colorId;
            SizeId = sizeId;
            FlavourId = flavourId;
        }

        public int ProductId { get; set; }
      
        public int? ColorId { get; set; }
        public int? SizeId { get; set; }
        public int? FlavourId { get; set; }



    }
}
