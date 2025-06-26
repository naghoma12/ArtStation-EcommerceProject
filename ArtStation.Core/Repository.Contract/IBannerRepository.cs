using ArtStation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Repository.Contract
{
    public interface IBannerRepository:IGenericRepository<Banner>
    {
        Task<IEnumerable<Banner>> GetAllBannersSortedAsync();
    }
}
