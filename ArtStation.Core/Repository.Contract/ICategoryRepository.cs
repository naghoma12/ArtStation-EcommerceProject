using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Repository.Contract
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDTO>> GetAllCategories(string language);
        Task<CategoryWithProducts> GetCategoryById(string language , int id , int? userId);
    }
}
