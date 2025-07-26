using ArtStation.Core;
using ArtStation.Core.Entities.Order;
using ArtStation.Core.Helper.Order;
using ArtStation.Core.Services.Contract;
using ArtStation_Dashboard.ViewModels.Order;
using ArtStation_Dashboard.ViewModels.User;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ArtStation_Dashboard.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IOrderService orderService,IMapper mapper,IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
           _mapper = mapper;
           _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderVM>>> Index(int page = 1, int pageSize = 6)
        {
            try
            {

                var result = await _orderService.GetOrdersDashboardAsync(page,pageSize);

               
                var totalPages = (int)Math.Ceiling(result.TotalItems / (double)pageSize);

                
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;

           
                var mappedOrders = _mapper.Map<IEnumerable<OrderVM>>(result.Items);

                return View(mappedOrders);
            }
            catch (Exception ex)
            {
              
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحميل الطلبات.";
                return View(new List<OrderVM>());
            }
        }

        [HttpGet]
        public async Task<ActionResult<Order>> Details(int id)
        {
            try
            {
                var order = await _orderService.GetOrderWithDetailsDashboardAsync(id);
                if (order == null)
                {
                    TempData["ErrorMessage"] = "الطلب غير موجود.";
                    return NotFound();
                }
                var orderVM = _mapper.Map<OrderInvoiceVM>(order);
                return View(orderVM);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحميل تفاصيل الطلب.";
                return View(new Order());
            }
        }

        }
}
