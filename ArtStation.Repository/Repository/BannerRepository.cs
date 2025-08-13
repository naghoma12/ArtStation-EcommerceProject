using ArtStation.Core;
using ArtStation.Core.Entities;
using ArtStation.Core.Repository.Contract;
using ArtStation.Repository.Data;
using ArtStation_Dashboard.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Repository.Repository
{
    public class BannerRepository : GenericRepository<Banner>, IBannerRepository
    {
        public BannerRepository(ArtStationDbContext context) : base(context)
        {

        }
        public async Task<IEnumerable<Banner>> GetAllBannersSortedAsync()
        {
            var banners = await _context.Banners.Where(b => b.IsActive == true)
                 .OrderBy(b => b.OrderBanner)
                 .ToListAsync();
            return banners;
        }

        public async Task<PagedResult<Banner>> GetBannerswithStatusAsync(int page, int pageSize, bool? statusFilter)
        {
            var query = _context.Banners
                .Where(o => !o.IsDeleted); // Always exclude deleted

            if (statusFilter.HasValue)
            {
                query = query.Where(o => o.IsActive == statusFilter.Value);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (totalPages == 0) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var items = await query
                .OrderByDescending(o => o.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Banner>
            {
                TotalItems = totalItems,
                PageNumber = page,
                PageSize = pageSize,
                Items = items,
                 TotalPages = totalPages
            };
        }

    }
}
