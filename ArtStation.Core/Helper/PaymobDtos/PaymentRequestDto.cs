using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Helper.PaymobDtos
{
    public class PaymentRequestDto
    {
            public int AmountCents { get; set; }           
            public string Currency { get; set; } = "EGP";  

            public string? Email { get; set; }             
            public string FullName { get; set; }         
                  
            public string Phone { get; set; }             

            public PaymentType PaymentType { get; set; }        

            public List<ItemDto> Items { get; set; } = new();  
        }
    
}
