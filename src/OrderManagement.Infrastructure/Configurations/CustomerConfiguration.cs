using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FullName).IsRequired();
        
        builder.OwnsOne(c => c.CpfCnpj, document =>
        {
            document.Property(d => d.Value)
                .HasColumnName("CpfCnpj")
                .IsRequired();
        });
    }
}