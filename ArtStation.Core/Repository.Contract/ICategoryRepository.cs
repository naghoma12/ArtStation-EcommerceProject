using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
using ArtStation_Dashboard.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Repository.Contract
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategories(string language);
        Task<CategoryWithProducts> GetCategoryById(string language , int id , int? userId);
        Task<IEnumerable<SimpleCategoryDTO>> GetSimpleCategory(string language);
        Task<PagedResult<Category>> GetFilteredAsync(
    Expression<Func<Category, bool>> filter = null,
    int page = 1,
    int pageSize = 5);
        Task<CategoryProductsPagination> GetCategoryWithProducts(string language, int id,
        int? userId = null, int page = 1, int pageSize = 3);
    }
}
