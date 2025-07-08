using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.PaymobDtos
{
    public class ItemDto
    {
        public string ProductName { get; set; }
        public decimal AmountCent { get; set; }
        public int Quantity { get; set; }
    }
}
