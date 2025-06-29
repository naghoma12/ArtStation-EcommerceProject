using ArtStation.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Cart
{
    public class Cart
    {
       
            public Cart()
            {

            }

            public Cart(string cartId)
            {
            CartId = cartId;
                CartItems = new List<CartItem>();
           
            }

            public string CartId { get; set; } // Key
          
            public List<CartItem>? CartItems { get; set; } = new List<CartItem>(); 

            public int? AddressId { get; set; }
      

        //public string? PaymentId { get; set; }  //PaymentIntent
        //public string? ClientSecret { get; set; } //ClientSecret
    }

}
