using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Identity
{
    public class Address : BaseEntity
    {

        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public Shipping Shipping { get; set; }
        public int ShippingId { get; set; }
        public string AddressDetails { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Long { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
