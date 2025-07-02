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

namespace ArtStation.Services
{
    public class OrderService : IOrderService
    {
        private readonly IProductRepository _productRepo;
        private readonly ICartRepository _cartRepository;
        private readonly ICartService _cartService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository _orderRepo;

        public OrderService(IProductRepository productRepo,ICartRepository cartRepository, ICartService cartService, IUnitOfWork unitOfWork, IOrderRepository orderRepo)
        {
           _productRepo = productRepo;
            _cartRepository = cartRepository;
            _cartService = cartService;
            _unitOfWork = unitOfWork;
            _orderRepo = orderRepo;
        }
        public async Task<Order?> CreateOrderAsync(string CustomerPhone, string CartId, int AddressId)
        {
            //1.Get Cart from cart repo
            var cart = await _cartRepository.GetCartAsync(CartId);
            //var cartData = await _cartService.MapCartToReturnDto(cart, CultureInfo.CurrentUICulture.Name);
            //2 Get OrderItems  fro product 
            var OrderItems = new List<OrderItem>();

            if (cart?.CartItems?.Count() > 0)
            {
                foreach (var item in cart.CartItems)
                {
                    ProductItemDetails productDetails = new ProductItemDetails();
                    var Photos = string.Empty;

                    var product = await _productRepo.GetProductWithPrice(item.ProductId, (int)item.SizeId);
                    productDetails = new ProductItemDetails(product.Product.Id,item.ColorId,item.SizeId,item.FlavourId);

                    var orderitem = new OrderItem(productDetails, item.Quantity,1); //static userid
                    //var orderitem = new OrderItem(productDetails, item.Quantity, product.UserId);
                 
                    orderitem.TotalPrice = (decimal)(product.PriceAfterSale == 0 ? product.Price * item.Quantity : product.PriceAfterSale * item.Quantity);

                    OrderItems.Add(orderitem);

                }
            }

            //calc subtotal

            var TotalPrice = OrderItems.Sum(OI => OI.TotalPrice);
           
            //createorder

            var order = new Order(CustomerPhone,AddressId, TotalPrice, OrderItems);
            //save to db
            _unitOfWork.Repository<Order>().Add(order);
            var rows = await _unitOfWork.Complet();
            if (rows <= 0)
                return null;

            return order;


        }


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


    }
}
