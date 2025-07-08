using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Payment
{
    public enum PaymentType
    {
        [EnumMember(Value = "card")]
        Card,
        [EnumMember(Value = "wallet")]
        Wallet,
        [EnumMember(Value = "cashOnDelivery")]
        CashOnDelivery
    }
}
