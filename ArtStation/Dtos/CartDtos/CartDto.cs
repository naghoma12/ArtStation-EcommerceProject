using ArtStation.Core.Entities.Cart;

namespace ArtStation.Dtos.CartDtos
{
    public class CartDto
    {
        public string Id { get; set; } 

        public List<CartItemDto>? CartItems { get; set; } = new List<CartItemDto>();
      
    }
}
