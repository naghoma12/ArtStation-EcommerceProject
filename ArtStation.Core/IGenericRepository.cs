using ArtStation.Core.Entities;
using ArtStation_Dashboard.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
        Task<PagedResult<T>> GetAllAsync(int page , int pageSize);
        Task<T> GetByIdAsync(int id);

        void Add(T entity);

        void Update(T entity);
        void Delete(T entity);
        Task UpdateRange(IEnumerable<T> entities);

    }
}
