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
        private readonly IProductRepository _productRepository;

        public CartRepository(IConnectionMultiplexer redis,IAddressRepository addressRepository
            , IMapper mapper,IUnitOfWork unitOfWork,IProductRepository productRepository)
        {
            _database = redis.GetDatabase();
           _addressRepository = addressRepository;
            _mapper = mapper;
           _unitOfWork = unitOfWork;
           _productRepository = productRepository;
        }
        public async Task<Cart?> GetCartAsync(string cartId)
        {
            var CustomerCart = await _database.StringGetAsync(cartId);
            return CustomerCart.IsNullOrEmpty ? null : JsonSerializer.Deserialize<Cart>(CustomerCart);
        }

        public async Task<Cart> AddCartAsync(Cart cart)
        {
           
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

        public async Task<Cart> DeleteItemAsync(string cartId,string id)
        {
            var cart=await GetCartAsync(cartId);
           
            cart.CartItems.Remove(cart.CartItems.FirstOrDefault(x => x.ItemId == id));

            if(cart.CartItems.Count == 0)
            {
                await DeleteCartAsync(id);
                return null;
            }
            var customercart = await _database.StringSetAsync(cart.Id, JsonSerializer.Serialize(cart), TimeSpan.FromDays(2));
            if (customercart is false)
            {
                return null;
            }
            return cart is null ? null : await AddCartAsync(cart);
        }

        public async Task<Cart> ChooseDeliveryAddress(string cartId, int addressId)
        {

            var cart =await GetCartAsync(cartId);
           
            cart.AddressId = addressId;
            var customercart = await _database.StringSetAsync(cart.Id, JsonSerializer.Serialize(cart), TimeSpan.FromDays(2));
            if (customercart is false)
            {
                return null;
            }
            return cart;
        }
    }
}
