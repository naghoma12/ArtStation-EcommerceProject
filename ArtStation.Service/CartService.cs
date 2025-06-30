using ArtStation.Core.Entities.Cart;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities;
using ArtStation.Core.Helper;
using ArtStation.Core.Repository.Contract;
using ArtStation.Core;
using ArtStation.Dtos.CartDtos;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtStation.Core.Services.Contract;

namespace ArtStation.Services
{
    public class CartService:ICartService
    {
        private readonly IMapper _mapper;
        private readonly IAddressRepository _addressRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IProductRepository _productRepository;

        public CartService(IMapper mapper,
            IAddressRepository addressRepository, IUnitOfWork unitOfWork,
            IProductRepository productRepository
            )
        {

            _mapper = mapper;
            _addressRepository = addressRepository;
            this.unitOfWork = unitOfWork;
            _productRepository = productRepository;
        }
        public async Task<CartReturnDto> MapCartToReturnDto(Cart cart, string? lang)
        {
            CartReturnDto cartReturnDto = new CartReturnDto();
            Address address = null;
            Shipping shippingCity = new Shipping();

            if (cart.AddressId > 0)
            {
                address = await _addressRepository.GetAddressWithShipping((int)cart.AddressId);
                //shippingCity = await unitOfWork.Repository<Shipping>().GetByIdAsync(address.ShippingId);

                cartReturnDto.Address = _mapper.Map<DeliveryAddress>(address);
                //cartReturnDto.Address.City = shippingCity.City;
            }

            cartReturnDto.Id = cart.CartId;

            foreach (var item in cart.CartItems)
            {
                var product = await _productRepository.GetProductById(lang, item.ProductId, null);
                if (product != null)
                {
                    var selectedSize = product.Sizes.FirstOrDefault(s => s.Id == item.SizeId);
                    var price = selectedSize?.Price ?? 0;
                    var priceAfterSale = selectedSize?.PriceAfterSale;

                    cartReturnDto.CartItems.Add(new CartItemReturnDto
                    {
                        ItemId = item.ItemId,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Price = price,
                        PriceAfterSale = priceAfterSale,
                        Quantity = item.Quantity,
                        PhotoUrl = product.Images.FirstOrDefault(),
                        Flavour = product.Flavours.FirstOrDefault(f => f.Id == item.FlavourId)?.Name,
                        Size = selectedSize?.Size,
                        Color = product.Colors.FirstOrDefault(c => c.Id == item.ColorId)?.ColorName
                    });
                }
            }

            var shippingCost = shippingCity?.Cost ?? 0.0m;

            cartReturnDto.CartSummary = new CartSummary
            {
                TotalItems = cartReturnDto.CartItems?.Count ?? 0,
                TotalPriceBeforeDiscount = cartReturnDto.CartItems?.Sum(item => item.Price * item.Quantity) ?? 0,
                ShippingPrice = shippingCost,
                TotalPriceAfterDiscount = cartReturnDto.CartItems?
                    .Sum(item => (item.PriceAfterSale == 0 || item.PriceAfterSale == null
                        ? item.Price : item.PriceAfterSale.Value) * item.Quantity) ?? 0,
                FinalTotal = (
                    cartReturnDto.CartItems?
                        .Sum(item => (item.PriceAfterSale == 0 || item.PriceAfterSale == null
                            ? item.Price : item.PriceAfterSale.Value) * item.Quantity) ?? 0
                ) + shippingCost
            };

            return cartReturnDto;
        }
    }
}
