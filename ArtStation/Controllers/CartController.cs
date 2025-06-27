using ArtStation.Core.Entities.Cart;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Repository.Contract;
using ArtStation.Core.Roles;
using ArtStation.Dtos.CartDtos;
using ArtStation.Repository.Repository;

using ArtStation.Core.Resources;
using AutoMapper;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Security.Claims;
using ArtStation.Core;
using ArtStation.Core.Entities;
using System.Globalization;
using ArtStation.Core.Services.Contract;
using Twilio.TwiML.Messaging;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private readonly ICartService _cartService;

        public CartController(ICartRepository cartRepository
            ,UserManager<AppUser> userManager,IMapper mapper,
           ICartService cartService
            )
        {
            _cartRepository = cartRepository;
           _userManager = userManager;
            _mapper = mapper;
            _cartService = cartService;
        }

        [HttpGet("GetBeforeAdd")]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                var userId = User.Identity.IsAuthenticated
                    ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    : null;

                Cart cart;

                if (userId != null)
                {
                    cart = await _cartRepository.GetCartAsync($"UserCart:{userId}");
                    if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                    {
                        return Ok(new
                        {
                            message = ControllerMessages.CartEmpty,
                            data = new Cart($"UserCart:{userId}")
                        });
                    }

                    return Ok(new
                    {
                        message = ControllerMessages.GetCartSucessfully,
                        data = cart
                    });
                }

                else
                {
                    return BadRequest(new
                    {
                        message = ControllerMessages.CartLoadedFailed,
                        data  = (object?)null
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ControllerMessages.CartLoadedFailed,
                    data = (object?)null
                });
            }
        }


        [HttpGet("GetAllCartData")]
        public async Task<IActionResult> GetAllCartData()
        {
            try
            {
                var userId = User.Identity.IsAuthenticated
                    ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    : null;

                Cart cart;

                if (userId != null)
                {
                    cart = await _cartRepository.GetCartAsync($"UserCart:{userId}");
                    if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                    {
                        return Ok(new
                        {
                            message = ControllerMessages.CartEmpty, 
                            data = new Cart($"UserCart:{userId}")
                        });
                    }
                    var cartReturnDto=await _cartService.MapCartToReturnDto(cart, CultureInfo.CurrentCulture.Name);
                    return Ok(new
                    {
                        message = ControllerMessages.GetCartSucessfully,
                        data = cartReturnDto
                    });
                }
               
                else
                {
                    return BadRequest(new
                    {
                        message = ControllerMessages.CartLoadedFailed ,
                        data = new[] { "User is not authenticated." }
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ControllerMessages.CartLoadedFailed, 
                    data = (object?)null
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddCart([FromBody] CartDto cartdto)
        {
            try
            {
                var cart = _mapper.Map<Cart>(cartdto);
                 var createOrUpdateBasket = await _cartRepository.AddCartAsync(cart);
                return Ok(new
                {
                    message = ControllerMessages.ItemAddedSuccessfully,
                    data = createOrUpdateBasket
                });
              
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    message = ControllerMessages.ItemFailedToAddedInCart,
                    data = (object?)null
                });
            }
        }

        [HttpDelete("DeleteCart")]
        public async Task<IActionResult> DeleteCartAsync([FromQuery] string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new {message= ControllerMessages.FailedToDeleteCart ,
                        data = (object?)null
                    });
                }

               
                bool deleted = await _cartRepository.DeleteCartAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = ControllerMessages.FailedToDeleteCart , 
                        data = (object?)null });
                }

                return Ok(new {message=ControllerMessages.CartClearedSuccessfully,
                    data = (object?)null
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    message = ControllerMessages.FailedToDeleteCart,
                    data = (object?)null
                });
            }
        }


        [HttpDelete("DeleteItem")]
        public async Task<IActionResult> DeleteItemAsync([FromQuery] string cartId, [FromQuery] string itemId)
        {
            try
            {
                if (string.IsNullOrEmpty(cartId) || string.IsNullOrEmpty(itemId))
                {
                    return BadRequest(new {message=ControllerMessages.CartItemDeletedFailed,
                        data = (object?)null
                    });
                }
               
                var updatedCart = await _cartRepository.DeleteItemAsync(cartId,itemId);
                
                if (updatedCart == null)
                {
                    return NotFound(new { message = ControllerMessages.CartItemDeletedFailed ,
                        data = (object?)null
                    });
                }
                var cartReturnDto = await _cartService.MapCartToReturnDto(updatedCart, CultureInfo.CurrentUICulture.Name);
                return Ok(new
                {
                    message = ControllerMessages.CartItemDeletedSucessfully,
                    data = cartReturnDto
                });
              
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ControllerMessages.CartItemDeletedFailed,
                    data = (object?)null
                });
            }

        }

        [HttpPost("ChooseDeliveryAddress")]
        public async Task<IActionResult> ChooseDeliveryAddress([FromQuery] string cartId, [FromQuery] int addressId)
        {
            try
            {
                if (string.IsNullOrEmpty(cartId) || addressId <= 0)
                {
                    return BadRequest(new {message=ControllerMessages.ChooseAddressDeliveryFailed,
                
                         data = (object?)null
                    
                });
                }
                   var updatedCart = await _cartRepository.ChooseDeliveryAddress(cartId, addressId);
                if (updatedCart == null)
                {
                    return NotFound(new { message = ControllerMessages.ChooseAddressDeliveryFailed,
             
                         data = (object?)null
                   
                });
                }
                var cartReturnDto = await _cartService.MapCartToReturnDto(updatedCart, CultureInfo.CurrentUICulture.Name);
                return Ok(new
                {
                     message = ControllerMessages.ChooseAddressDeliverySucessfully,
                    data = cartReturnDto
                });

               
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ControllerMessages.ChooseAddressDeliveryFailed,
                 
                     data = (object?)null
                 
            });
            }
        }

       
    }


}
