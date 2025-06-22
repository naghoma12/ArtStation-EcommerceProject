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
            var MappedList = _mapper.Map<IEnumerable<Category>, IEnumerable<CategoryDTO>>(list);
            return Ok(MappedList);
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<CategoryWithProducts>> GetById([FromHeader] string language, int id, int? userId)
        {
            var category = await _categoryRepository.GetCategoryById(language, id, userId);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }
    }
}
