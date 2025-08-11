using ArtStation.Core.Entities;
using ArtStation_Dashboard.ViewModels;
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

        Task<PagedResult<Banner>> GetBannerswithStatusAsync(int page, int pageSize, bool? statusFilter);
    }
}
