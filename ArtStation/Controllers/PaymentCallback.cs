using ArtStation.Core.Entities.Order;
using ArtStation.Core;
using ArtStation.Dtos.PaymobDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ArtStation.Core.Entities;

namespace ArtStation.Controllers
{
    [ApiController]
    [Route("api/payment/callback")]
    public class PaymentCallbackController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentCallbackController> _logger;

        public PaymentCallbackController(IUnitOfWork unitOfWork, ILogger<PaymentCallbackController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        //[HttpPost]
        //public async Task<IActionResult> HandleCallback([FromBody] PaymobCallbackDto dto)
        //{
        //    try
        //    {
        //        var transaction = dto.obj.transaction;
        //        var paymobOrderId = dto.obj.order.id;

        //        // لو العملية ناجحة وتم سحب الأموال
        //        if (transaction.success && transaction.is_captured)
        //        {
        //            // ابحث عن الطلب برقم paymobOrderId داخل الـ PaymentTransaction (أو اربطه بجدول Order لو عندك الربط المباشر)
        //            var existing = await _unitOfWork.Repository<PaymentTransaction>()
        //                .FindAsync(t => t.TransactionId == transaction.id);

        //            if (existing != null)
        //            {
        //                _logger.LogInformation($"Callback already handled for transaction {transaction.id}");
        //                return Ok(); // رجع 200 عشان Paymob ما يكررش الإرسال
        //            }

        //            // إنشاء معاملة جديدة
        //            var newTransaction = new PaymentTransaction
        //            {
        //                TransactionId = transaction.id,
        //                OrderId = paymobOrderId,
        //                Amount = transaction.amount_cents / 100m,
        //                Currency = transaction.currency,
        //                Status = "Paid",
        //                PaymentDate = DateTime.UtcNow,
        //                PaymentMethod = "paymob"
        //            };

        //            _unitOfWork.Repository<PaymentTransaction>().Add(newTransaction);

        //            // تحديث حالة الطلب في جدول الطلبات
        //            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(paymobOrderId);
        //            if (order != null)
        //            {
        //                order.Status = OrderStatus.Paid;
        //            }

        //            await _unitOfWork.Complet();
        //            _logger.LogInformation($"Payment completed for order #{paymobOrderId}");
        //        }

        //        return Ok(); // مهم جدًا ترجع 200 دائمًا حتى لو فشل التحقق
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while handling Paymob callback");
        //        return Ok(); // لا ترجع BadRequest حتى لا يعيد Paymob المحاولة عشوائيًا
        //    }
        //}
    }

}
