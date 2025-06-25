using ArtStation.Core;
using ArtStation.Core.Entities;
using ArtStation.Core.Entities.Cart;
using ArtStation.Core.Repository.Contract;
using AutoMapper;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArtStation.Repository.Repository
{
    public class CartRepository:ICartRepository
    {
        private readonly IDatabase _database;
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CartRepository(IConnectionMultiplexer redis,IAddressRepository addressRepository
            , IMapper mapper,IUnitOfWork unitOfWork)
        {
            _database = redis.GetDatabase();
           _addressRepository = addressRepository;
            _mapper = mapper;
           _unitOfWork = unitOfWork;
        }
        public async Task<Cart?> GetCartAsync(string cartId)
        {
            var CustomerCart = await _database.StringGetAsync(cartId);
            return CustomerCart.IsNullOrEmpty ? null : JsonSerializer.Deserialize<Cart>(CustomerCart);
        }

        public async Task<Cart> AddCartAsync(Cart cart)
        {
            //var address=await _addressRepository.GetByIdAsync((int)cart.AddressId);
            cart.CartSummary = new CartSummary
            {
                TotalItems = cart.CartItems?.Count ?? 0,
                TotalPriceBeforeDiscount = cart.CartItems?.Sum(item => item.Price * item.Quantity) ?? 0,
                ShippingPrice = 0.0M, 
                TotalPriceAfterDiscount = cart.CartItems?.Sum(item => item.PriceAfterSale * item.Quantity) ?? 0,
                FinalTotal = (cart.CartItems?.Sum(item => item.PriceAfterSale??item.Price * item.Quantity) ?? 0) + 0.0M 
            };
            var customercart = await _database.StringSetAsync(cart.Id, JsonSerializer.Serialize(cart), TimeSpan.FromDays(2));
            if (customercart is false)
            {
                return null;
            }
            return await GetCartAsync(cart.Id);
        }

        public async Task<bool> DeleteCartAsync(string cartId)
        {
            return await _database.KeyDeleteAsync(cartId);

        }

        public async Task<Cart> DeleteItemAsync(string id)
        {
            var cart=await GetCartAsync(id);
            cart.CartItems.Remove(cart.CartItems.FirstOrDefault(x => x.ItemId == id));
            return cart is null ? null : await AddCartAsync(cart);
        }

        public async Task<Cart> ChooseDeliveryAddress(string cartId, int addressId)
        {

            var cart =await GetCartAsync(cartId);
            var address= _addressRepository.GetByIdAsync(addressId).Result;
            var shipping = await _unitOfWork.Repository<Shipping>().GetByIdAsync(address.ShippingId);
            cart.Address = _mapper.Map<DeliveryAddress>(address);
            cart.CartSummary.ShippingPrice = shipping.Cost;
            cart.Address.City = shipping.City;
            return cart;
        }
    }
}
