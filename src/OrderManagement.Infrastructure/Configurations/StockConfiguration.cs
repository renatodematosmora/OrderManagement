using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Configurations;

public class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.ToTable("Stocks");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ProductId).IsRequired();
        builder.Property(s => s.Quantity).IsRequired();

        builder.HasOne<Product>()
        .WithOne()
        .HasForeignKey<Stock>(s => s.ProductId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.ProductId).IsUnique();
    }
}