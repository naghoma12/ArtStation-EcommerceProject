using ArtStation.Core.Entities;
using ArtStation.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtStation.Repository.Data;
using Microsoft.EntityFrameworkCore;
using ArtStation_Dashboard.ViewModels;

namespace ArtStation.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private protected readonly ArtStationDbContext _context;

        public GenericRepository(ArtStationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<T>> GetAllAsync(int page , int pageSize)
        {
            var query = _context.Set<T>().AsNoTracking();

            var totalItems = await query.CountAsync();

            var items = await query
                .Where(z => z.IsDeleted == false 
                && z.IsActive == true)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
               

            return new PagedResult<T>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>()
            .Where(z => z.IsDeleted == false 
            && z.IsActive == true 
            && z.Id == id).FirstOrDefaultAsync();
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _context.Update(entity);
        }
        public void Delete(T entity)
        {
            _context.Remove(entity);
        }
    }
}
