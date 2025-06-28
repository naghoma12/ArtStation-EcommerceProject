using ArtStation.Core.Entities.Order;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Repository.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(OItem => OItem.ProductItem, ProductItem => ProductItem.WithOwner());

            builder.Property(o => o.OrderItemStatus).HasConversion(
                ItemStatus => ItemStatus.ToString(), //store
                   ItemStatus => (OrderItemStatus)Enum.Parse(typeof(OrderItemStatus), ItemStatus)//retrive
                 );
            builder.Property(o => o.TotalPrice).HasColumnType("decimal(18,2)");
           
            //builder.Property(o => o.ProductOrderDetails.NormalPrice).HasColumnType("decimal(18,2)");
            //builder.Property(o => o.ProductOrderDetails.PriceAfterSale).HasColumnType("decimal(18,2)");
        }
    }
}
