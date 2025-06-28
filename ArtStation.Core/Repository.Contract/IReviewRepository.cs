using ArtStation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Repository.Contract
{
    public interface IReviewRepository
    {
        Task<bool> IsLiked(int userId , int reviewId);
        Task<ReviewLikes> GetReviewLike(int userId , int reviewId);

    }
}
