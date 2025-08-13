using ArtStation.Core.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Entities.Order
{
    public enum OrderItemStatus
    {

        [Display(Name = "Placed", ResourceType = typeof(Messages))]
        Placed,
        [Display(Name = "Shipped", ResourceType = typeof(Messages))]
        Shipped,
        [Display(Name = "Delivered", ResourceType = typeof(Messages))]
        Delivered,
     
    }

}
