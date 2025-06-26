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
                if (userId == null)
                {
                    return BadRequest("There is no Users with this Id"); // localization
                }
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
                if (product == null)
                {
                    return BadRequest("There is no Products with this Id"); // localization
                }
                var Fav = await _fav.GetFavoriteAsync(productId, userId);
                if (Fav != null)
                {
                    return BadRequest("هذا المنتج مضاف الى المفضله بالفعل"); // localization
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
                            Message = $"تم إضافة المنتج إلى المفضلة بنجاح" // localization
                        });
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = $"فشلت عملية الإضافه {ex.Message.ToString() ?? ex.InnerException?.Message.ToString()}"
                });
            }

        }
        [HttpPost("DeleteFavourite")]
        public async Task<IActionResult> DeleteFavourite(int productId)
        {
            try
            {
                var user = await _user.GetUserAsync(User);
                if (user == null)
                {
                    return BadRequest("لا يوجد مستخدم بهذا الكود");
                }
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
                if (product == null)
                {
                    return BadRequest("لا يوجد منتج بهذا الكود");
                }
                var Fav = await _fav.GetFavoriteAsync(productId, user.Id);
                if (Fav != null)
                {
                    _unitOfWork.Repository<Favourite>().Delete(Fav);
                    var count = await _unitOfWork.Complet();
                    if (count > 0)
                    {
                        return Ok(new 
                        {
                            Message = $"تم حذف المنتج من المفضله بنجاح"
                        });
                    }
                }
                return BadRequest("هذا المنتج غير موجود في المفضله");
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                {
                    Message = $"فشلت عملية حذف المنتج من المفضله {ex.Message.ToString() ?? ex.InnerException?.Message.ToString()}"
                });
            }

        }

        [HttpGet("GetAllFavorites")]
        public async Task<IActionResult> GetAllFavouriteProducts()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                if (userId == null)
                {
                    return BadRequest("لا يوجد مستخدم بهذا الكود"); //localization
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
