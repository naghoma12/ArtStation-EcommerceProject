using ArtStation.Core;
using ArtStation.Core.Entities;
using ArtStation.Core.Repository.Contract;
using ArtStation.Core.Resources;
using ArtStation.DTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtStation.Controllers
{
    [Authorize]
    public class ReviewController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReviewRepository _reviewRepository;

        public ReviewController(IUnitOfWork unitOfWork, IReviewRepository reviewRepository)
        {
            _unitOfWork = unitOfWork;
            _reviewRepository = reviewRepository;
        }
        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview(UserReview userReview)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    Review review = new Review()
                    {
                        Comment = userReview.Comment,
                        Rating = userReview.Rating,
                        ProductId = userReview.ProductId,
                        UserId = userId,
                        LikesCount = 0
                    };
                    _unitOfWork.Repository<Review>().Add(review);
                    var count = await _unitOfWork.Complet();
                    if (count > 0)
                    {
                        return Ok(new { Message = ControllerMessages.ReviewAdded });
                    }
                }
                catch
                {
                    return BadRequest(new { Message = ControllerMessages.ReviewNotAdded });
                }
            }
            return BadRequest(new { Message = ControllerMessages.ReviewNotAdded });
        }

        [HttpGet("IsLiked")]
        public async Task<IActionResult> IsLiked(int reviewId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var isLiked = await _reviewRepository.IsLiked(userId, reviewId);

            return Ok(new { Message = isLiked ? "User is Liked on this review" : "User is not liked on this review", IsLiked = isLiked });
        }
        [HttpPost("AddLike")]
        public async Task<IActionResult> AddLike(int reviewId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var review = await _unitOfWork.Repository<Review>().GetByIdAsync(reviewId);
            if (review == null)
            {
                return BadRequest(new { Message = ControllerMessages.ReviewNotFound });
            }
            var isLiked = await _reviewRepository.IsLiked(userId, reviewId);
            if (isLiked)
            {
                return BadRequest(new { Message = ControllerMessages.ReviewLiked });
            }
            ReviewLikes reviewLike = new ReviewLikes()
            {
                UserId = userId,
                ReviewId = reviewId
            };
            _unitOfWork.Repository<ReviewLikes>().Add(reviewLike);
            _unitOfWork.Complet();
            review.LikesCount += 1;
            _unitOfWork.Repository<Review>().Update(review);
            _unitOfWork.Complet();
            return Ok(new { Message = ControllerMessages.LikeAdded });
        } 

        [HttpDelete("DeleteLike")]
        public async Task<IActionResult> DeleteLike(int reviewId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var review = await _unitOfWork.Repository<Review>().GetByIdAsync(reviewId);
            if (review == null)
            {
                return BadRequest(new { Message = ControllerMessages.ReviewNotFound });
            }
            var isLiked = await _reviewRepository.IsLiked(userId, reviewId);
            if (!isLiked)
            {
                return BadRequest(new { Message = ControllerMessages.ReviewNotLiked });
            }

            var reviewLike = await _reviewRepository.GetReviewLike(userId, reviewId);
                _unitOfWork.Repository<ReviewLikes>().Delete(reviewLike);
                review.LikesCount -= 1;
                _unitOfWork.Repository<Review>().Update(review);
                await _unitOfWork.Complet();
                return Ok(new { Message = ControllerMessages.LikeDeleted });
            
        }

        [HttpPut("UpdateReview")]
        public async Task<IActionResult> UpdateReview(UserReview userReview , int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    var review = await _unitOfWork.Repository<Review>().GetByIdAsync(id);
                    if (review == null || review.UserId != userId)
                    {
                        return BadRequest(new { Message = ControllerMessages.ReviewNotFound });
                    }
                    review.Comment = userReview.Comment;
                    review.Rating = userReview.Rating;
                    review.ProductId = userReview.ProductId;
                    review.UserId = userId;
                    review.LikesCount = review.LikesCount; 
                    _unitOfWork.Repository<Review>().Update(review);
                    var count = await _unitOfWork.Complet();
                    if (count > 0)
                    {
                        return Ok(new { Message = ControllerMessages.ReviewUpdated });
                    }
                }
                catch
                {
                    return BadRequest(new { Message = ControllerMessages.ReviewNotUpdated });
                }
            }
            return BadRequest(new { Message = ControllerMessages.ReviewNotUpdated });
        }

        [HttpDelete("DeleteReview")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var review = await _unitOfWork.Repository<Review>().GetByIdAsync(id);
            if (review == null || review.UserId != userId)
            {
                return BadRequest(new { Message = ControllerMessages.ReviewNotFound });
            }
            _unitOfWork.Repository<Review>().Delete(review);
            var count = await _unitOfWork.Complet();
            if (count > 0)
            {
                return Ok(new { Message = ControllerMessages.ReviewDeleted });
            }
            return BadRequest(new { Message = ControllerMessages.ReviewNotDeleted });
        }
    }
}
