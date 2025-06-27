//using ArtStation.Core.Entities.Identity;
//using ArtStation.Core.Entities.Order;
//using ArtStation.Core.Entities;
//using ArtStation.Core.Repository.Contract;
//using ArtStation.Core.Services.Contract;
//using ArtStation.Core;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Globalization;

//namespace ArtStation.Services
//{
//    public class OrderService : IOrderService
//    {
//        private readonly ICartRepository _cartRepository;
//        private readonly ICartService _cartService;
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IOrderRepository _orderRepo;

//        public OrderService(ICartRepository cartRepository,ICartService cartService, IUnitOfWork unitOfWork, IOrderRepository orderRepo)
//        {
//            _cartRepository = cartRepository;
//            _cartService = cartService;
//            _unitOfWork = unitOfWork;
//            _orderRepo = orderRepo;
//        }
//        public async Task<Order?> CreateOrderAsync(string CustomerEmail, string CartId, int shippingCostId, Address ShippingAddress)
//        {
//            //1.Get Cart from cart repo
//            var cart = await _cartRepository.GetCartAsync(CartId);
//            var cartData = await _cartService.MapCartToReturnDto(cart, CultureInfo.CurrentUICulture.Name);
//            //2 Get OrderItems  fro product 
//            var OrderItems = new List<OrderItem>();

//            if (cart?.CartItems?.Count() > 0)
//            {
//                foreach (var item in cartData.CartItems)
//                {
//                    ProductItemDetails productDetails = new ProductItemDetails();
//                    var Photos = string.Empty;
                   
//                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
                   
//                    productDetails = new ProductItemDetails(product.Id,produc) ;
                    
//                    var orderitem = new OrderItem(productDetails, item.Quantity, product.UserId);
//                    orderitem.TotalPrice = (decimal)(productDetails.PriceAfterSale == 0 ? productDetails.Price * item.Quantity : productDetails.PriceAfterSale * item.Quantity);

//                    OrderItems.Add(orderitem);

//                }
//            }

//            //calc subtotal

//            var TotalPrice = OrderItems.Sum(OI => OI.TotalPrice);
//            //get shippingcost
//            var ShippingCost = await _unitOfWork.Repository<Shipping>().GetByIdAsync(shippingCostId);
//            //createorder

//            var order = new Order(CustomerEmail, ShippingAddress, TotalPrice, OrderItems, ShippingCost);
//            //save to db
//            _unitOfWork.Repository<Order>().Add(order);
//            var rows = await _unitOfWork.Complet();
//            if (rows <= 0)
//                return null;

//            return order;


//        }

//        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string Email)
//        {
//            var orders = await _orderRepo.GetUserOrdersAsync(Email);

//            return (IReadOnlyList<Order>)orders;
//        }

//        public async Task<Order> GetOrderForUserAsync(int orderid)
//        {
//            var order = await _orderRepo.GetOrderForUserAsync(orderid);

//            return order;
//        }

       

      



     
     
//    }
//}
