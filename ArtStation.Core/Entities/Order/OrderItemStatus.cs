using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Order
{
    public enum OrderItemStatus
    {
      
        [EnumMember(Value = "قيد المعاينة")]
        Pending,
        [EnumMember(Value = "قيد التنفيذ")]
        InProgress,
        [EnumMember(Value = "تم التجهيز")]
        Ready,
        [EnumMember(Value = "تم إالغاء")]
        Cancelled
    }

}
