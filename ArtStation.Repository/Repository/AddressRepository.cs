using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Repository.Contract;
using ArtStation.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Repository.Repository
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(ArtStationDbContext dbContext):base(dbContext)
        {
            
        }
        public async Task<IEnumerable<Address?>> GetAllUserAddress(int userId)
        {
            var addresses = await _context.Address.Where(a => a.AppUserId == userId).ToListAsync();
            return addresses ?? new List<Address>();
        }
    }
}
