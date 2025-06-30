using ArtStation.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Repository.Contract
{
    public interface IAddressRepository:IGenericRepository<Address>
    {
        public Task<IEnumerable<Address?>> GetAllUserAddress(int userId);
        public Task<Address?> GetAddressWithShipping(int userId);
    }
}
