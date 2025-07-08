using ArtStation.Core.Entities.Order;
using ArtStation.Core;
using ArtStation.Dtos.PaymobDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ArtStation.Core.Entities;
using ArtStation.Core.Entities.Payment;
using Microsoft.AspNetCore;
using ArtStation.Repository;
using System.Text.Json;

namespace ArtStation.Controllers
{
    [ApiController]
    [Route("api")]
    public class WebhookController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(IUnitOfWork unitOfWork, ILogger<WebhookController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

      

[HttpPost("webhook/paymob")]
    public async Task<IActionResult> HandleCallback()
    {
        try
        {
            using var reader = new StreamReader(Request.Body);
            var rawJson = await reader.ReadToEndAsync();

            _logger.LogInformation("Webhook Raw JSON: {json}", rawJson);

            
            var dto = JsonSerializer.Deserialize<PaymobCallbackDto>(rawJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (dto == null || dto.Obj == null)
            {
                _logger.LogWarning("Webhook payload is null or invalid.");
                return BadRequest("Invalid JSON payload.");
            }

            var transaction = dto.Obj;
            var paymobOrderId = transaction.Order.Id;
            var transactionId = transaction.Id.ToString();

            if (transaction.Success && transaction.IsCapture && !transaction.IsRefunded)
            {
                var existing = await _unitOfWork.Repository<PaymentTransaction>()
                    .FindAsync(t => t.TransactionId == transactionId);

                if (existing != null)
                {
                    _logger.LogInformation($" Callback already handled for transaction {transactionId}");
                    return Ok();
                }

                var paymentMethod = transaction.SourceData?.Type ?? "paymob";

                var newTransaction = new PaymentTransaction
                {
                    TransactionId = transactionId,
                    OrderId = paymobOrderId.ToString(),
                    Amount = transaction.AmountCents / 100m,
                    Currency = transaction.Currency,
                    Status = "Paid",
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = paymentMethod
                };

                _unitOfWork.Repository<PaymentTransaction>().Add(newTransaction);

                var order = await _unitOfWork.Repository<Order>()
                    .FindAsync(o => o.PaymobOrderId == paymobOrderId);

                if (order != null)
                {
                    order.PaymentStatus = PaymentStatus.Paid;
                    order.Status = OrderStatus.Placed;
                    await _unitOfWork.Complet();

                    _logger.LogInformation($"Payment confirmed for Order #{order.Id} (Transaction {transactionId})");
                }
                else
                {
                    _logger.LogWarning($"Order not found for PaymobOrderId: {paymobOrderId}");
                }
            }
            else
            {
                _logger.LogWarning($"Transaction failed or not captured. Transaction ID: {transactionId}");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling Paymob webhook");
            return BadRequest("Exception: " + ex.Message);
        }
    }

   
}

}
