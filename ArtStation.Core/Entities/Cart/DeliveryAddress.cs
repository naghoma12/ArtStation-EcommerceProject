using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Messaging;

namespace ArtStation.Core.Entities.Cart
{
    public class DeliveryAddress
    {
       
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string AddressDetails { get; set; }
        public int ShippingId { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Long { get; set; }
    }
}
