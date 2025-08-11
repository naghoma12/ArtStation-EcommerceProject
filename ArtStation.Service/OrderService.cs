using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities.Order;
using ArtStation.Core.Entities;
using ArtStation.Core.Repository.Contract;
using ArtStation.Core.Services.Contract;
using ArtStation.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using ArtStation.Core.Helper;
using Twilio.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ArtStation.Core.Entities.Payment;
using ArtStation.Core.Entities.PaymobDtos;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ArtStation_Dashboard.ViewModels;
using ArtStation.Core.Helper.Order;
using Microsoft.EntityFrameworkCore;

namespace ArtStation.Services
{
    public class OrderService : IOrderService
    {
        private readonly IProductRepository _productRepo;
        private readonly ICartRepository _cartRepository;
        private readonly ICartService _cartService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _config;

        public OrderService(IProductRepository productRepo,ICartRepository cartRepository, 
            ICartService cartService, IUnitOfWork unitOfWork, IOrderRepository orderRepo,
            UserManager<AppUser> userManager
            ,IPaymentService paymentService,
             IConfiguration config
            )
        {
           _productRepo = productRepo;
            _cartRepository = cartRepository;
            _cartService = cartService;
            _unitOfWork = unitOfWork;
            _orderRepo = orderRepo;
            _userManager = userManager;
            _paymentService = paymentService;
           _config = config;
        }
        //public async Task<(Order? order, string? redirectUrl, string? paymentToken)> CreateOrderAsync(AppUser user, string CartId, int AddressId, string paymentType)
        //{
        //    //1.Get Cart from cart repo
        //    var cart = await _cartRepository.GetCartAsync(CartId);
        //    //var cartData = await _cartService.MapCartToReturnDto(cart, CultureInfo.CurrentUICulture.Name);
        //    //2 Get OrderItems  fro product 
        //    var OrderItems = new List<OrderItem>();

        //    if (cart?.CartItems?.Count() > 0)
        //    {
        //        foreach (var item in cart.CartItems)
        //        {
        //            ProductItemDetails productDetails = new ProductItemDetails();
        //            var Photos = string.Empty;

        //            var product = await _productRepo.GetProductWithPrice(item.ProductId, (int)item.SizeId);
        //            productDetails = new ProductItemDetails(product.Product.Id, item.ColorId > 0 ? item.ColorId : null, item.SizeId, item.FlavourId > 0 ? item.FlavourId : null);

        //            //var orderitem = new OrderItem(productDetails, item.Quantity,1); //static userid
        //            var orderitem = new OrderItem(productDetails, item.Quantity, product.UserId);

        //            orderitem.TotalPrice = (decimal)(product.PriceAfterSale == 0 ? product.Price * item.Quantity : product.PriceAfterSale * item.Quantity);

        //            OrderItems.Add(orderitem);

        //        }
        //    }

        //    //3. calc subtotal

        //    var TotalPrice = OrderItems.Sum(OI => OI.TotalPrice);


        //    //5. createorder

        //    var order = new Order(user.PhoneNumber, AddressId, TotalPrice, OrderItems);
        //    //  الدفع عبر Paymob
        //    string? redirectUrl = null;
        //    string? paymentToken = null;

        //    if (paymentType.ToLower() != "cash")
        //    {
        //        var totalCents = (int)(TotalPrice * 100);
        //        var token = await _paymentService.AuthenticateAsync();

        //        var itemTasks = OrderItems.Select(async i =>
        //        {
        //            //var product = await _productRepo.GetProductById(CultureInfo.CurrentUICulture.Name, i.ProductItem.ProductId, null);
        //            return new ItemDto
        //            {
        //                ProductName = "product?.Name",
        //                AmountCent = (int)(i.TotalPrice * 100),
        //                Quantity = i.Quantity
        //            };
        //        });

        //        var itemDtos = (await Task.WhenAll(itemTasks)).ToList();
        //        if (!Enum.TryParse<PaymentType>(paymentType, true, out var paymentTypeEnum))
        //        {
        //            throw new ArgumentException("نوع الدفع غير مدعوم");
        //        }
        //        var dto = new PaymentRequestDto
        //        {
        //            AmountCents = totalCents,
        //            Currency = "EGP",
        //            Email = string.IsNullOrEmpty(user.Email) ? "no@email.com" : user.Email,
        //            FullName = user.FullName,
        //            Phone = user.PhoneNumber,
        //            PaymentType = paymentTypeEnum,
        //            Items = itemDtos
        //        };

        //        var paymobOrderId = await _paymentService.CreateOrderAsync(token, dto);
        //        paymentToken = await _paymentService.GeneratePaymentKeyAsync(token, paymobOrderId, dto);
        //        order.PaymentToken = paymentToken;

        //        order.PaymobOrderId = paymobOrderId;
        //        order.PaymentMethod = paymentTypeEnum;

        //        switch (paymentType.ToLower())
        //        {
        //            case "card":
        //                redirectUrl = $"https://accept.paymob.com/api/acceptance/iframes/{_config["Paymob:IframeId"]}?payment_token={paymentToken}";
        //                break;

        //            case "wallet":
        //                // يتم فتح هذه الصفحة ليدخل المستخدم رقم هاتفه ويختار شبكة المحفظة
        //                redirectUrl = $"https://accept.paymob.com/api/acceptance/iframes/937215?payment_token={paymentToken}";
        //                break;

        //            case "cash":
        //            default:
        //                redirectUrl = null;
        //                break;
        //        }
        //    }

        //    //// 3. حفظ الـ token
        //    //order.PaymentId = paymentToken;


        //    //save to db
        //    _unitOfWork.Repository<Order>().Add(order);
        //    var rows = await _unitOfWork.Complet();
        //    if (rows <= 0)
        //        return (null, null, null);

        //    return (order, redirectUrl, paymentToken);


        //}


        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string PhoneNumber)
        {
            var orders = await _orderRepo.GetUserOrdersAsync(PhoneNumber);

            return (IReadOnlyList<Order>)orders;
        }

        public async Task<OrderWithItemsDto> GetOrderForUserAsync(int orderid)
        {
            var order = await _orderRepo.GetOrderForUserAsync(orderid);

            return order;
        }

        //public async Task<PagedResult<Order>> GetOrdersDashboardAsync(int page, int pageSize)
        //{
        //    return await _unitOfWork.Repository<Order>().GetAllAsync(page, pageSize);
        //}

        public async Task<PagedResult<Order>> GetOrdersDashboardAsync(int page, int pageSize, string statusFilter)
        {
            var query = await _unitOfWork.Repository<Order>().GetAllAsync();

            if (!string.IsNullOrEmpty(statusFilter))
                query = query.Where(o => o.Status.ToString() == statusFilter);

            var totalItems = query.Count();
            var items =  query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<Order>
            {
                TotalItems = totalItems,
                Items = items
            };
        }


        public async Task<OrderInvoiceDto> GetOrderWithDetailsDashboardAsync(int id)
        {
            return await _orderRepo.GetOrderWithDetailsAsync(id);
        }
    }
}
