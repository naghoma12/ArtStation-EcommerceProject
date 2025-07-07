using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtStation.Core.Entities.Order;
using System.Reflection.Emit;

namespace ArtStation.Repository.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {

            builder.Property(o => o.Status).HasConversion(
                           order=>order.ToString(), //store
                           order=> (OrderStatus)Enum.Parse(typeof(OrderStatus), order)//retrive
            );


              builder.Property(o => o.PaymentMethod)
                 .HasConversion<string>();
            builder.Property(o => o.PaymentStatus)
               .HasConversion<string>();
          
            builder.Property(o => o.SubTotal).HasColumnType("decimal(18,2)");

        }
    }
}
