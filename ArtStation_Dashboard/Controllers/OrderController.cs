using ArtStation.Core.Services.Contract;
using ArtStation_Dashboard.ViewModels.Order;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ArtStation_Dashboard.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService,IMapper mapper)
        {
            _orderService = orderService;
           _mapper = mapper;
        }

        public async Task<ActionResult<IEnumerable<OrderVM>>> Index() 
        {
            var orders = await _orderService.GetOrdersDashboardAsync();

            var mappedOrders= _mapper.Map<IEnumerable<OrderVM>>(orders);
            return View(mappedOrders);
        }
    }
}
