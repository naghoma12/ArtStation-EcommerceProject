using ArtStation.Core;
using ArtStation.Core.Entities.Order;
using ArtStation.Core.Helper.Order;
using ArtStation.Core.Roles;
using ArtStation.Core.Services.Contract;
using ArtStation_Dashboard.Helper;
using ArtStation_Dashboard.Resource;
using ArtStation_Dashboard.ViewModels;
using ArtStation_Dashboard.ViewModels.Order;
using ArtStation_Dashboard.ViewModels.User;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtStation_Dashboard.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IOrderService orderService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderVM>>> Index(int page = 1, int pageSize = 6, string statusFilter = null)
        {
            try
            {
                var result = await _orderService.GetOrdersDashboardAsync(page, pageSize, statusFilter); // مرر الفلتر

                var totalPages = (int)Math.Ceiling(result.TotalItems / (double)pageSize);

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageSize = pageSize;
                ViewBag.StatusFilter = statusFilter;

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


        //[HttpPost]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReadyOrder(int orderid)
        {
            try
            {
                var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderid);
                if (order == null)
                {
                    TempData["ErrorMessage"] = "الطلب غير موجود.";
                    return NotFound();
                }
                order.Status = OrderStatus.Shipped;
                _unitOfWork.Repository<Order>().Update(order);
                await _unitOfWork.Complet();
                //TempData["SuccessMessage"] = ViewMessages.OrderReady;

                
                return Ok();

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحديث حالة الطلب.";
                return RedirectToAction("Details", new { id = orderid });
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
            if (order == null)
                return Json(new { success = false, message = "Not found" });

            if (!Enum.TryParse<OrderStatus>(status, true, out var newStatus))
                return Json(new { success = false, message = "Invalid status value" });

            order.Status = newStatus;
            await _unitOfWork.Complet();
            return Json(new { success = true, status = newStatus.ToString() });
        }

        ////EndPoints For Get Specific Orders For Company

        [Authorize(Roles = Roles.Trader)]
        [HttpGet]
        public ActionResult OrderSections()
        {
            return View();
        }
        [Authorize(Roles = Roles.Trader)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyOrderVM>>> CompanyOrder(
           int page = 1,
           int pageSize =2,
           string statusFilter = null,
           string search = null)
        {
            ViewBag.StatusFilter = statusFilter;
            ViewBag.Search = search;
            try
            {
                // Get Trader ID from Claims
                var traderIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(traderIdString) || !int.TryParse(traderIdString, out var traderId))
                {
                    TempData["ErrorMessage"] = "لم يتم العثور على معرف التاجر.";
                    return View(Enumerable.Empty<CompanyOrderVM>());
                }

                // Get filtered + paginated orders for this trader
                var result = await _orderService.GetOrdersForCompanyAsync(traderId, page, pageSize, statusFilter);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    if (int.TryParse(search.Trim(), out int searchNum))
                    {
                        int targetId = searchNum - 1000;

                        result.Items = result.Items
                            .Where(o => o.Id == targetId)
                            .ToList();
                    }
                }
                // Pagination after filtering
                var totalItems = result.TotalItems;
                //ViewBag.TotalItems = totalItems;
                //ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                //ViewBag.CurrentPage = page;

                //var pagedOrders = result.Items
                //    .Skip((page - 1) * pageSize)
                //    .Take(pageSize)
                //    .ToList();
                var list = new PagedResult<Order>
                {
                    Items = result.Items,
                    TotalItems = totalItems,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalItems / pageSize)
                };
                // Return partial for AJAX
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return PartialView("_CompanyOrderTable",list);

                return View(list);
                
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error loading company orders");
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحميل الطلبات.";
                return View(Enumerable.Empty<CompanyOrderVM>());
            }
        }
        //GetInvoiceForCompany
        [HttpGet]
        public async Task<ActionResult<Order>> InvoiceForCompany(int orderId)
        {
            try
            {
                var traderIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(traderIdString) || !int.TryParse(traderIdString, out var traderId))
                {
                    TempData["ErrorMessage"] = "لم يتم العثور على معرف التاجر.";
                    return View(Enumerable.Empty<CompanyOrderVM>());
                }
                var order = await _orderService.GetInvoiceForCompanyAsync(orderId,traderId);
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

        //Make Ready 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReadyOrderForCompany(int orderid)
        {
            try
            {
                // Get Trader ID from Claims
                var traderIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(traderIdString) || !int.TryParse(traderIdString, out var traderId))
                {
                    TempData["ErrorMessage"] = "لم يتم العثور على معرف التاجر.";
                    return View(Enumerable.Empty<CompanyOrderVM>());
                }
                var order = await _orderService.ReadyOrderForCompanyAsync(orderid,traderId);
                if (order == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "الطلب غير موجود."
                    });
                }

                if (order == "Not Ready Order")
                {
                    return Json(new
                    {
                        success = false,
                        message = "لا يمكن تجهيز الطلب في حالته الحالية."
                    });
                }
                var orderCheck= await _orderService.GetOrderWithItemsAsync(orderid);
                if (orderCheck.OrderItems.All(oi=>oi.OrderItemStatus==OrderItemStatus.Shipped))
                {
                    orderCheck.Status = OrderStatus.Shipped;
                    _unitOfWork.Complet();
                }
                return Json(new
                {
                    success = true,
                    message = "تم تجهيز الطلب بنجاح!"
                });
            }

            
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحديث حالة الطلب.";
                return RedirectToAction("Details", new { id = orderid });
            }
        }

    }

}
