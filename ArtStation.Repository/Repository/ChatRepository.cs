using ArtStation.Core.Entities.AI;
using ArtStation.Core.Helper;
using ArtStation.Core.Helper.AiDtos;
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
    public class ChatRepository : GenericRepository<RecommendedProduct>, IChatRepository
    {
        private readonly ArtStationDbContext _context;

        public ChatRepository(ArtStationDbContext context):base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ChatResponse>> ChatResponses(int userId)
        {
            return await _context.Recommendations
                .Where(x => x.IsActive && !x.IsDeleted && x.UserId == userId)
                .OrderByDescending(x => x.CreatedDate)
                .Select(p => new ChatResponse
                {
                    Note = p.Note,
                    Image = string.IsNullOrEmpty(p.Image) ? null :
                $"http://artstation.runasp.net//Images//ChatResponseImages/{p.Image}",
                    Message = p.Message,
                    UserId = userId,
                    Reply = p.Reply,
                    RecommendedProducts = p.RecommendedProducts.Select(
                        r => new AIProducts
                        {
                            Id = r.Id,
                            Name = r.Product.NameEN,
                        }).ToList()
                })
                .ToListAsync();
        }
    }
}
