using ArtStation.Core.Entities.Cart;

namespace ArtStation.Dtos.CartDtos
{
    public class CartDto
    {
        public string CartId { get; set; }

        public List<CartItemDto>? CartItems { get; set; } = new List<CartItemDto>();

        public int? AddressId { get; set; }
    }
}
