using ArtStation.Core.Entities.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Repository.Contract
{
    public  interface ICartRepository
    {
        Task<Cart> GetCartAsync(string id);
      
        Task<Cart> AddCartAsync(Cart cart);
        Task<bool> DeleteCartAsync(string id);
        Task<Cart> DeleteItemAsync(string cartId,string id);
    
        Task<Cart> ChooseDeliveryAddress(string cartId, int addressId);
    }
}
