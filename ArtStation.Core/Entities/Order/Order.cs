using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Order
{
    public class Order:BaseEntity
    {
        public Order()
        {

        }
        public Order(string customerPhone, int addressId, decimal subTotal, ICollection<OrderItem> orderItems)
        {
            CustomerPhone = customerPhone;
            AddressId = addressId;
            SubTotal = subTotal;
            OrderItems = orderItems;
          
        }

        public string CustomerPhone { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Placed;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public PaymentType PaymentMethod { get; set; }

        public int AddressId { get; set; }
        public Address Address { get; set; }
        public decimal SubTotal { get; set; }
        //public decimal GetTotal() => SubTotal + Shipping.Cost;
        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        //  رقم الطلب في Paymob (من عملية إنشاء order)
        public int? PaymobOrderId { get; set; }

        //  توكن الدفع (payment_token) - تستخدمه لإنشاء رابط الدفع
        public string PaymentToken { get; set; }

        // رقم المعاملة بعد الدفع (يصل في callback)
        public int? PaymobTransactionId { get; set; }

    }
}
