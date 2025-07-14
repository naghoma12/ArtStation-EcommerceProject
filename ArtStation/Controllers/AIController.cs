using ArtStation.Core;
using ArtStation.Core.Entities;
using ArtStation.Core.Entities.AI;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Helper;
using ArtStation.Core.Helper.AiDtos;
using ArtStation.Core.Repository.Contract;
using ArtStation.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArtStation.Controllers
{

    public class AIController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IChatRepository _chatRepository;
        private readonly IScanRepository _scanRepository;

        public AIController(IUnitOfWork unitOfWork
            , UserManager<AppUser> userManager
            , IChatRepository chatRepository,
            IScanRepository scanRepository)
        {
           _unitOfWork = unitOfWork;
           _userManager = userManager;
           _chatRepository = chatRepository;
            _scanRepository = scanRepository;
        }

        [HttpPost("SaveChatResponse")]
        public async Task<IActionResult> SaveChatResponse(ChatResponse chatResponse)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(chatResponse.UserId.ToString());
                    if (user == null)
                    {
                        return NotFound(new { Message = "User not found." });
                    }
                    if (chatResponse.PhotoFile != null)
                    {
                        chatResponse.Image = HandlerPhoto.UploadPhoto(chatResponse.PhotoFile, "ChatResponseImages");
                    }
                    ChatResponseRecommendation chatResponseRecommendation = new ChatResponseRecommendation()
                    {
                        Note = chatResponse.Note,
                        Message = chatResponse.Message,
                        Reply = chatResponse.Reply,
                        UserId = chatResponse.UserId,
                        Image = chatResponse.Image
                    };
                    _unitOfWork.Repository<ChatResponseRecommendation>().Add(chatResponseRecommendation);
                   var count = await _unitOfWork.Complet();
                    if(count > 0)
                    {
                        if (chatResponse.RecommendedProducts != null && chatResponse.RecommendedProducts.Any())
                        {
                             foreach (var product in chatResponse.RecommendedProducts)
                             {
                            var exsistProduct = await _unitOfWork.Repository<Product>().GetByIdAsync((int)product.Id);
                            if (exsistProduct == null)
                            {
                                return NotFound(new { Message = "Product not found." });
                            }   
                            RecommendedProduct recommendedProduct = new RecommendedProduct()
                            {
                                ProductId = (int)product.Id,
                                ChatResponseRecommendationId = chatResponseRecommendation.Id
                            };
                            _unitOfWork.Repository<RecommendedProduct>().Add(recommendedProduct);
                             }
                             await _unitOfWork.Complet();
                        }
                        
                        
                    }
                    return Ok(new { Message = "Chat response saved successfully." });
                }
                catch
                {
                    return BadRequest(new
                    {
                        Message = "An error occurred while saving the chat response. Please try again later."
                    });
                }
            }
                return BadRequest(new
                {
                    Message = "An error occurred while saving the chat response. Please try again later."
                });
        }

        [HttpGet("GetChatHistory")]
        public async Task<IActionResult> GetChatHistory(string token)
        {
            var userId = Utility.GetUserId(token);
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }
            var chatResponses = await _chatRepository.ChatResponses(userId);
            if (chatResponses == null || !chatResponses.Any())
            {
                return Ok(new { Message = "No chat history found." });
            }
            return Ok(new { Message = "Chat history retrieved successfully.", ChatHistory = chatResponses });
        }

        [HttpPost("SaveFaceScan")]
        public async Task<IActionResult> SaveFaceScan(FaceScan faceScan)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(faceScan.UserId.ToString());
                    if (user == null)
                    {
                        return NotFound(new { Message = "User not found." });
                    }
                    if (faceScan.PhotoFile != null)
                    {
                        faceScan.Image = HandlerPhoto.UploadPhoto(faceScan.PhotoFile, "FaceScanImages");
                    }
                    SkinAnalysis skinAnalysis = new SkinAnalysis()
                    {
                        IsFaceDetected = faceScan.IsFaceDetected,
                        SkinScore = faceScan.SkinScore,
                        SkinAge = faceScan.SkinAge,
                        FinalNote = faceScan.FinalNote,
                        Image= faceScan.Image,
                        UserId = faceScan.UserId
                    };
                    _unitOfWork.Repository<SkinAnalysis>().Add(skinAnalysis);
                    var count = await _unitOfWork.Complet();
                    if (count > 0)
                    {
                        foreach (var metric in faceScan.Metrics)
                        {
                            SkinMetric skinMetric = new SkinMetric()
                            {
                                ProblemName = metric.ProblemName,
                                Score = metric.Score,
                                Comment = metric.Comment,
                                SkinAnalysisId = skinAnalysis.Id
                            };
                            _unitOfWork.Repository<SkinMetric>().Add(skinMetric);
                        }
                        var result = await _unitOfWork.Complet();
                        return Ok(new { Message = "Face scan saved successfully." });
                    }
                }
                catch
                {
                    return BadRequest(new
                    {
                        Message = "An error occurred while saving the face scan. Please try again later."
                    });
                }
            }
            return BadRequest(new
            {
                Message = "An error occurred while saving the face scan. Please try again later."
            });
        }

        [HttpGet("ScanHistory")]
        public async Task<IActionResult> ScanHistory(string token)
        {
            var userId = Utility.GetUserId(token);
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }
            var scanHistory = await _scanRepository.GetScanHistory(userId);
            if (scanHistory == null || !scanHistory.Any())
            {
                return Ok(new { Message = "No scan history found." });
            }
            return Ok(new { Message = "Scan history retrieved successfully.", ScanHistory = scanHistory });
        }
    }
}
