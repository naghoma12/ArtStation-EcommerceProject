using ArtStation.Core;
using ArtStation.Core.Entities;
using ArtStation.Dtos.AuthDtos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace ArtStation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly Core.IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShippingController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
           _mapper = mapper;
        }

        [HttpGet]
        [Route("GetAllCities")]
        public async Task<ActionResult<IEnumerable<ShippingDto>>> GetAllShippingAddresses()
        {
            var shippingAddresses =await _unitOfWork.Repository<Shipping>().GetAllAsync();
            var cities= _mapper.Map<IEnumerable<Shipping>, IEnumerable<ShippingDto>>(shippingAddresses);
            return Ok(cities);
        }

      
    }
}
