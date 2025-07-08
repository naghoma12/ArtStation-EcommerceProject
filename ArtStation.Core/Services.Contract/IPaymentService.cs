using ArtStation.Core.Entities.PaymobDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Services.Contract
{
    public interface IPaymentService
    {
        public Task<string> AuthenticateAsync();
        public Task<int> CreateOrderAsync(string token, PaymentRequestDto dto);
        public Task<string> GeneratePaymentKeyAsync(string token, int orderId, PaymentRequestDto dto);
    }
}
