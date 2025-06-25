using ArtStation.Core.Entities.Cart;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Repository.Contract;
using ArtStation.Core.Roles;
using ArtStation.Dtos.CartDtos;
using ArtStation.Repository.Repository;
using ArtStation.Resources;
using AutoMapper;
using FirebaseAdmin.Messaging;
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
     
        [HttpDelete("DeleteCart")]
        public async Task<IActionResult> DeleteCartAsync([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new {message= ControllerMessages.FailedToDeleteCart });
                }

               
                bool deleted = await _cartRepository.DeleteCartAsync(id);
                if (!deleted)
                {
                    return NotFound(ControllerMessages.FailedToDeleteCart);
                }

                return Ok(new {message=ControllerMessages.CartClearedSuccessfully});
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
                    return BadRequest(new {message=ControllerMessages.CartItemDeletedFailed});
                }
                // Delete the item from the cart
                var updatedCart = await _cartRepository.DeleteItemAsync(id);
                if (updatedCart == null)
                {
                    return NotFound(new { message = ControllerMessages.CartItemDeletedFailed });
                }
                return Ok(new {
                    message = ControllerMessages.CartItemDeletedSucessfully ,
                    updatedCart}
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ControllerMessages.CartItemDeletedFailed });
            }

        }

        [HttpPost("ChooseDeliveryAddress")]
        public async Task<IActionResult> ChooseDeliveryAddress([FromQuery] string cartId, [FromQuery] int addressId)
        {
            try
            {
                if (string.IsNullOrEmpty(cartId) || addressId <= 0)
                {
                    return BadRequest(new {message=ControllerMessages.ChooseAddressDeliveryFailed});
                }
                // Choose the delivery address for the cart
                var updatedCart = await _cartRepository.ChooseDeliveryAddress(cartId, addressId);
                if (updatedCart == null)
                {
                    return NotFound(new { message = ControllerMessages.ChooseAddressDeliveryFailed });
                }
                return Ok(new { message = ControllerMessages.ChooseAddressDeliverySucessfully ,
                    updatedCart });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ControllerMessages.ChooseAddressDeliveryFailed });
            }
        }
        }
}
