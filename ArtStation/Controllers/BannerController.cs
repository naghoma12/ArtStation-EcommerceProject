using ArtStation.Core.Repository.Contract;
using ArtStation.Core;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ArtStation.Dtos;
using ArtStation.Resources;

namespace ArtStation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBannerRepository _bannerRepository;

        public BannerController(IUnitOfWork unitOfWork, IMapper mapper, IBannerRepository bannerRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _bannerRepository = bannerRepository;
        }

        // Get All Active Banners Ordered
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BannerDto>>> GetAllBanners()
        {
            try
            {
                var banners = await _bannerRepository.GetAllBannersSortedAsync();
                if (banners == null || !banners.Any())
                {
                    return NotFound(new { message = ControllerMessages.BannersNotFound, data = (object?)null });
                }

                var mappedBanners = _mapper.Map<IEnumerable<BannerDto>>(banners);
                return Ok(new { message = ControllerMessages.BannersSucess, data = mappedBanners });
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, new { message = ControllerMessages.BannersNotFound, error = ex.Message });
            }
        }

    }
}
