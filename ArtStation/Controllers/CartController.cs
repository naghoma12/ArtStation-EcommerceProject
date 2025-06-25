using ArtStation.Core.Entities.Cart;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Repository.Contract;
using ArtStation.Core.Roles;
using ArtStation.Dtos.CartDtos;
using ArtStation.Repository.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Security.Claims;

namespace ArtStation.Controllers
{

    [Authorize(Roles = Roles.Customer)]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IAddressRepository _addressRepository;

        public CartController(ICartRepository cartRepository
            ,UserManager<AppUser> userManager,IMapper mapper,
            IAddressRepository addressRepository)
        {
            _cartRepository = cartRepository;
           _userManager = userManager;
            _mapper = mapper;
            _addressRepository = addressRepository;
        }
  
        [HttpGet]
        public async Task<IActionResult> GetCart(string? cartId)
        {
            var userId = User.Identity.IsAuthenticated
                ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;
         
            var cart = new Cart();
            if (userId != null)
            {
                cart = await _cartRepository.GetCartAsync($"UserCart:{userId}");
                if (cart == null)
                {
                    return Ok(new Cart($"UserCart:{userId}"));
                }
            }
            else
            {
                cart = await _cartRepository.GetCartAsync(cartId);
                if (cart == null)
                {
                    return Ok(new Cart(cartId));
                }
            }

            return Ok(cart);
        }


        [HttpPost]
        public async Task<IActionResult> AddCart([FromBody]CartDto cartDto)
        {
            //var address=await _addressRepository.GetByIdAsync((int)cartDto.AddressId);
            //var userId = _userManager.GetUserId(User);
            //var cartadd = await _cartRepository.GetCartAsync(userId);
            var cart =_mapper.Map<Cart>(cartDto);
            //cart.Address = address;
            var CreateOrUpdateBasket = await _cartRepository.AddCartAsync(cart);
           
            return Ok(CreateOrUpdateBasket);

         

        }
     
        [HttpDelete("DeleteCartAsync")]
        public async Task<IActionResult> DeleteCartAsync([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("The cartId field is required.");
                }

                // Delete the cart from Redis (or your database)
                bool deleted = await _cartRepository.DeleteCartAsync(id);
                if (!deleted)
                {
                    return NotFound("Cart not found or could not be deleted.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("DeleteItem")]
        public async Task<IActionResult> DeleteItemAsync([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("The itemId field is required.");
                }
                // Delete the item from the cart
                var updatedCart = await _cartRepository.DeleteItemAsync(id);
                if (updatedCart == null)
                {
                    return NotFound("Item not found in the cart.");
                }
                return Ok(updatedCart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost("ChooseDeliveryAddress")]
        public async Task<IActionResult> ChooseDeliveryAddress([FromQuery] string cartId, [FromQuery] int addressId)
        {
            try
            {
                if (string.IsNullOrEmpty(cartId) || addressId <= 0)
                {
                    return BadRequest("The cartId and addressId fields are required.");
                }
                // Choose the delivery address for the cart
                var updatedCart = await _cartRepository.ChooseDeliveryAddress(cartId, addressId);
                if (updatedCart == null)
                {
                    return NotFound("Cart not found or could not update delivery address.");
                }
                return Ok(updatedCart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        }
}
