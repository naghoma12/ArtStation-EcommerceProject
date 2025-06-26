using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities;
using ArtStation.Core.Repository.Contract;
using ArtStation.Core;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ArtStation.DTOS;

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
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAll([FromHeader] string language)
        {
            var list = await _categoryRepository.GetAllCategories(language);
            if (list == null || !list.Any())
            {
                return Ok(new {
                    Message = "No categories found." ,
                    List = list
                });
            }
            return Ok(new
            {
                Message = "List of Categories .",
                List = list
            });
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<CategoryWithProducts>> GetById([FromHeader] string language, int id, int? userId)
        {
            var category = await _categoryRepository.GetCategoryById(language, id, userId);
            if (category == null)
            {
                return NotFound(new { Message = $"There is no category with this ID : {id}" });
            }
            return Ok(new
            {
                Message = "Category found successfully.",
                Category = category
            });
        }
    }
}
