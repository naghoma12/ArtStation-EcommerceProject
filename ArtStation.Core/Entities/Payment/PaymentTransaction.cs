using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Payment
{
    public class PaymentTransaction : BaseEntity
    {
        public string OrderId { get; set; }                 // رقم الطلب في تطبيقك
        public string TransactionId { get; set; }           // رقم المعاملة في Paymob
        public decimal Amount { get; set; }              // المبلغ المدفوع بالجنيه
        public string Currency { get; set; } = "EGP";    // العملة
        public string Status { get; set; }               // الحالة: Paid, Failed, etc.
        public string PaymentMethod { get; set; }        // مثل: Visa, Wallet, Meeza
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        // Navigation
        public Order.Order Order { get; set; }           // الربط بجدول الطلب
    }

}
