using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities.Order;
using ArtStation.Core.Helper;
using ArtStation.Core.Repository.Contract;
using ArtStation.Core.Resources;
using ArtStation.Core.Roles;
using ArtStation.Core.Services.Contract;
using ArtStation.Dtos.Order;
using ArtStation.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;
using OrderDto = ArtStation.Dtos.Order.OrderDto;

namespace ArtStation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartRepository _cartRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<OrderController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ICartService _cartService;
        private readonly string? _imagepath;

        public OrderController(IOrderService orderService
            , ICartRepository cartRepository
            , IAddressRepository addressRepository
            , IMapper mapper
            , UserManager<AppUser> userManager
            , IWebHostEnvironment webHostEnvironment,
            ILogger<OrderController> logger,
                        IConfiguration configuration,
                        ICartService cartService)
        {
            _orderService = orderService;
            _cartRepository = cartRepository;
            _addressRepository = addressRepository;
            _mapper = mapper;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _configuration = configuration;
           _cartService = cartService;
            _imagepath = _configuration["ApiBaseUrl"];
        }


        [Authorize(Roles =Roles.Customer)]
        [HttpPost]
      
        public async Task<ActionResult> CreateOrder(OrderDto orderDto)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized(new { Message = "المستخدم غير مصرح له" });
                }

                // استدعاء الخدمة مع إرجاع order + paymentToken + redirectUrl
                var (order, redirectUrl, paymentToken) = await _orderService.CreateOrderAsync(
                    user,
                    orderDto.CartId,
                    orderDto.AddressId,
                    orderDto.PaymentType
                );

                if (order == null)
                {
                    return BadRequest(new { Message = ControllerMessages.OrderFailed });
                }

                // حذف السلة بعد إنشاء الطلب
                var cart = await _cartRepository.GetCartAsync(orderDto.CartId);
                var orderData = await _cartService.MapCartToReturnDto(cart, "");
                await _cartRepository.DeleteCartAsync(orderDto.CartId);

                return Ok(new
                {
                    Message = ControllerMessages.OrderSuccesfully,
                    OrderId = order.Id,
                    PaymentType = orderDto.PaymentType,
                    PaymentToken = paymentToken,
                    PaymentUrl = redirectUrl,
                    Data = new
                    {
                        orderData.CartSummary,
                        orderData.Address
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating the order.");
                return BadRequest(new { Message = "حدث خطأ غير متوقع. يرجى المحاولة لاحقًا." });
            }
        }


        [Authorize]
        [HttpGet("UserOrders")]
        public async Task<ActionResult<IEnumerable<OrderReturnDto>>> GetUserOrders()
        {
            try
            {

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized(new { Message = ControllerMessages.Unauthorized,data=(object?)null });
                }


                var orders = await _orderService.GetOrdersForUserAsync(user.PhoneNumber);
                if (orders == null || !orders.Any())
                {
                    return NotFound(new { Message = ControllerMessages.NoOrdersForThisUser , data = (object?)null });
                }


                var mappedOrders = _mapper.Map<List<OrderReturnDto>>(orders);


                return Ok( new { message = ControllerMessages.UserHaveOrders,data= mappedOrders });
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error occurred while retrieving orders for user.");


                return BadRequest(new { message =ControllerMessages.SomethingWrong, data = (object?)null });
            }
        }


        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderWithItemsDto>> GetOrderForUser(int id)
        {
            try
            {

                var user = await _userManager.GetUserAsync(User);


                var order = await _orderService.GetOrderForUserAsync(id);


                if (order == null)
                {
                    _logger.LogWarning($"Order with id {id} not found for user {user?.PhoneNumber}");
                    return NotFound(new { Message = ControllerMessages.YourOrderNotExsist ,data=(object?)null });
                }

                return Ok(new { message = ControllerMessages.OrderIshere, data = order });
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"An error occurred while retrieving order with id {id}");


                return BadRequest(new { Message = ControllerMessages.SomethingWrong,data=(object?)null });
            }
        }
    }
}
