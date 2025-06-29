
using ArtStation.Core.Entities.Cart;
using ArtStation.Core.Helper;

namespace ArtStation.Dtos.CartDtos
{
    public class CartReturnDto
    {
        public string Id { get; set; }

        public List<CartItemReturnDto>? CartItems { get; set; } = new List<CartItemReturnDto>();
        public DeliveryAddress Address { get; set; }

        public CartSummary CartSummary { get; set; } = new CartSummary();
    }
}
