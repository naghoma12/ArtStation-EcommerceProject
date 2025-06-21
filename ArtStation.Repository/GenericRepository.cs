using ArtStation.Core.Entities;
using ArtStation.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtStation.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace ArtStation.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private protected readonly ArtStationDbContext _context;

        public GenericRepository(ArtStationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {

            return await _context.Set<T>()
                .Where(z => z.IsDeleted == false 
                && z.IsActive == true)
                .ToListAsync();
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
