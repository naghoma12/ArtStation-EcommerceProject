using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities;
using ArtStation.Core.Repository.Contract;
using ArtStation.Core;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ArtStation.Core.Resources;
using FirebaseAdmin.Messaging;

namespace ArtStation.Controllers
{
    [Authorize]
    public class FavouriteController : BaseController
    {

        private readonly IFavouriteRepository _fav;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _user;
        private readonly IMapper _mapper;

        public FavouriteController(IFavouriteRepository fav, IUnitOfWork unitOfWork
            , UserManager<AppUser> user, IMapper mapper)
        {
            _fav = fav;
            _unitOfWork = unitOfWork;
        }
        [HttpPost("AddFavourite")]
        public async Task<IActionResult> AddFavourite(int productId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                if (userId <= 0)
                {
                    return NotFound(new { Message = "Token not valid." });
                }
                if (userId == null)
                {
                    return BadRequest( new
                    {
                       Message = ControllerMessages.UserNotFound
                    }); 
                }
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
                if (product == null)
                {
                    return BadRequest(new { Message = ControllerMessages.ProductNotFound }); 
                }
                var Fav = await _fav.GetFavoriteAsync(productId, userId);
                if (Fav != null)
                {
                    return BadRequest(new { Message = ControllerMessages.FavProductExist}); // localization
                }
                else
                {
                    Favourite favorite = new Favourite()
                    {
                        ProductId = productId,
                        UserId = userId
                    };
                    _unitOfWork.Repository<Favourite>().Add(favorite);
                    var count = await _unitOfWork.Complet();
                    if (count > 0)
                    {
                        return Ok(new
                        {
                            Message = ControllerMessages.FavProductAdded // localization
                        });
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = $"Adding Failed {ex.Message.ToString() ?? ex.InnerException?.Message.ToString()}"
                });
            }

        }
        [HttpPost("DeleteFavourite")]
        public async Task<IActionResult> DeleteFavourite(int productId)
        {
            try
            {

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                if (userId <= 0)
                {
                    return NotFound(new { Message = "Token not valid." });
                }
                if (userId == null)
                {
                    return BadRequest(new { Message = ControllerMessages.UserNotFound });
                }
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
                if (product == null)
                {
                    return BadRequest(new {Message = ControllerMessages.ProductNotFound});
                }
                var Fav = await _fav.GetFavoriteAsync(productId, userId);
                if (Fav != null)
                {
                    _unitOfWork.Repository<Favourite>().Delete(Fav);
                    var count = await _unitOfWork.Complet();
                    if (count > 0)
                    {
                        return Ok(new 
                        {
                            Message = ControllerMessages.FavProductDeleted
                        });
                    }
                }
                return BadRequest(new
                {
                    Message = ControllerMessages.FavProductNotExist 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                {
                    Message = $"Deleting Failed {ex.Message.ToString() ?? ex.InnerException?.Message.ToString()}"
                });
            }

        }

        [HttpGet("GetAllFavorites")]
        public async Task<IActionResult> GetAllFavouriteProducts()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                if (userId <= 0)
                {
                    return NotFound(new { Message = "Token not valid." });
                }
                if (userId == null)
                {
                    return BadRequest(new
                    {
                        Message = ControllerMessages.UserNotFound
                    });
                }
                var language = Request.Headers["Accept-Language"].ToString();
                if (string.IsNullOrWhiteSpace(language) || (language != "en" && language != "ar"))
                    language = "en";
                var list = await _fav.FavouriteProducts(language, userId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = $"{ex.Message.ToString() ?? ex.InnerException?.Message.ToString()}"
                });
            }


        }


    }
}
