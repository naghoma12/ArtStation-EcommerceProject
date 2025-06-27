using ArtStation.Core.Entities.Cart;
using ArtStation.Dtos.CartDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Services.Contract
{
    public interface ICartService
    {
        public  Task<CartReturnDto> MapCartToReturnDto(Cart cart, string lang);
    }
}
