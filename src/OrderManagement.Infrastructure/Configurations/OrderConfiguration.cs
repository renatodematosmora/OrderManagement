using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.CustomerId).IsRequired();
        builder.Property(o => o.OrderDate).IsRequired();
        builder.Property(o => o.Status).IsRequired();

        // Amount é calculado (=> _orderItems.Aggregate(...)), não existe coluna pra ele
        builder.Ignore(o => o.OrderAmount);

        // Avisa o EF: "OrderItems" é lida através do campo privado _orderItems
        builder.Metadata
            .FindNavigation(nameof(Order.OrderItems))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<Customer>()
        .WithMany()
        .HasForeignKey(o => o.CustomerId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}