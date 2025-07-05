using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities
{
    public class PaymentTransaction
    {
        public int Id { get; set; } // مفتاح أساسي داخلي

        public int TransactionId { get; set; } // رقم المعاملة من Paymob
        public int OrderId { get; set; }       // رقم الطلب المرتبط

        public decimal Amount { get; set; }    // المبلغ المدفوع بالجنيه
        public string Currency { get; set; } = "EGP"; // العملة

        public string Status { get; set; } = "Pending"; // "Paid" - "Failed" - "Pending"
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        public string? PaymentMethod { get; set; } // مثل: "card", "wallet"

        // روابط (اختياري)
        public ArtStation.Core.Entities.Order.Order Order { get; set; } // العلاقة مع الطلب
    }

}
