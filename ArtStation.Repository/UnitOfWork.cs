using ArtStation.Core;
using ArtStation.Core.Entities;
using ArtStation.Repository.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Repository
{
    public class UnitOfWork : IUnitOfWork
    {

        private Hashtable _Repositories;
        private readonly ArtStationDbContext _context;

        public UnitOfWork(ArtStationDbContext Context)
        {
            _context = Context;
            _Repositories = new Hashtable();

        }
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity).Name;
            if (!_Repositories.ContainsKey(type))
            {
                var repository = new GenericRepository<TEntity>(_context);
                _Repositories.Add(type, repository);

            }
            return _Repositories[type] as IGenericRepository<TEntity>;
        }


        public async Task<int> Complet()
       => await _context.SaveChangesAsync();

        public async ValueTask DisposeAsync()
         => await _context.DisposeAsync();
    }
}
