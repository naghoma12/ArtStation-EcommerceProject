using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities;

namespace ArtStation.Dtos.UserDtos
{
    public class AddressDto
    {

        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
      
        public int ShippingId { get; set; }
        public string AddressDetails { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Long { get; set; }
       
       
    }
}
