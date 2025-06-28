using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities;
using ArtStation.Core.Repository.Contract;
using ArtStation.Core;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using ArtStation.Core.Helper;
using ArtStation.Core.Resources;

namespace ArtStation.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(IUnitOfWork unitOfWork
            , IMapper mapper
            ,ICategoryRepository categoryRepository)
        {
           
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAll()
        {
            var language = Request.Headers["Accept-Language"].ToString();

            if (string.IsNullOrWhiteSpace(language) || (language != "en" && language != "ar"))
                language = "en";
            var list = await _categoryRepository.GetAllCategories(language);
            if (list == null || !list.Any())
            {
                return Ok(new {
                    Message = ControllerMessages.CategoriesNotFound ,
                    List = list
                });
            }
            return Ok(new
            {
                Message = ControllerMessages.CategoriesFound,
                List = list
            });
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<CategoryWithProducts>> GetById(int id, string? token)
        {
            int? userId = Utility.CheckToken(token);
            var language = Request.Headers["Accept-Language"].ToString();
            if (string.IsNullOrWhiteSpace(language) || (language != "en" && language != "ar"))
                language = "en";
            var category = await _categoryRepository.GetCategoryById(language, id, userId);
            if (category == null)
            {
                return NotFound(new { Message = ControllerMessages.CategoryNotFound });
            }
            return Ok(new
            {
                Message = ControllerMessages.CategoryFound,
                Category = category
            });
        }
    }
}
