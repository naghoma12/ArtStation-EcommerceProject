using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Order
{
    public enum OrderStatus
    {
        [EnumMember(Value = "قيد المعاينة")]
        Pending,
		[EnumMember(Value = "قيد التنفيذ")]
        InProgress,
		[EnumMember(Value = "تم الشحن")]
        Shipping,
		[EnumMember(Value = "تم الاستلام")]
        Deliverd,
		[EnumMember(Value = "تم الالغاء")]
        Cancelled,
        [EnumMember(Value = "تم التجهيز")]
        Ready,
    }
}
