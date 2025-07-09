using ArtStation.Core.Entities.Payment;
using ArtStation.Core.Entities.PaymobDtos;
using ArtStation.Core.Services.Contract;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ArtStation.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public PaymentService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }
        public async Task<string> AuthenticateAsync()
        {
            var response = await _httpClient.PostAsJsonAsync($"{_config["Paymob:BaseUrl"]}/auth/tokens", new { api_key = _config["Paymob:ApiKey"] });
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PaymobAuthResponse>();
            return result.Token;
        }

        public async Task<int> CreateOrderAsync(string token, PaymentRequestDto dto)
        {
            var request = new
            {
                auth_token = token,
                delivery_needed = false,
                amount_cents = dto.AmountCents,
                currency = dto.Currency,
                items = dto.Items.Select(item => new
                {
                    name = item.ProductName,
                    amount_cents = item.AmountCent,
                    quantity = item.Quantity
                }).ToList()
            };

            var response = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/ecommerce/orders", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
            return result.Id; 
        }


        public async Task<string> GeneratePaymentKeyAsync(string token, int orderId, PaymentRequestDto dto)
        {
            if (dto.PaymentType == PaymentType.CashOnDelivery)
            {
                
                return null!;
            }
          
            int integrationId = dto.PaymentType switch
            {
                PaymentType.Card => int.Parse(_config["Paymob:CardIntegrationId"]),
                PaymentType.Wallet => int.Parse(_config["Paymob:WalletIntegrationId"]),
                _ => throw new Exception("Unsupported payment method")
            };

            var request = new
            {
                auth_token = token,
                amount_cents = dto.AmountCents,
                expiration = 3600,
                order_id = orderId,
                billing_data = new
                {
                    email = dto.Email,
                    first_name = dto.FullName?.Split(' ')[0] ?? "first",
                    last_name = dto.FullName?.Split(' ').Skip(1).FirstOrDefault() ?? "last",
                    phone_number = dto.Phone,
                    apartment = "NA",
                    floor = "NA",
                    street = "NA",
                    building = "NA",
                    city = "NA",
                    country = "NA",
                    state = "NA"
                },
                currency = dto.Currency,
                integration_id = integrationId
            };

            var response = await _httpClient.PostAsJsonAsync("https://accept.paymob.com/api/acceptance/payment_keys", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PaymentKeyResponse>();
            return result.Token;
        }


    }
}
