using ArtStation.Core.Entities;
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
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly ArtStationDbContext _context;

        public ReviewRepository(ArtStationDbContext context): base(context)
        {
            _context = context;
        }

        public async Task<ReviewLikes> GetReviewLike(int userId, int reviewId)
        {
            return _context.ReviewLikes
                 .Where(x => x.UserId == userId && x.ReviewId == reviewId
                 && !x.IsDeleted && x.IsActive).FirstOrDefault();
        }

        public async Task<bool> IsLiked(int userId, int reviewId)
        {
             var flag =  _context.ReviewLikes
                .Where(rl => rl.UserId == userId && rl.ReviewId == reviewId)
                .FirstOrDefault();
            if (flag != null)
                {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
