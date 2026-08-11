using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money Price { get; private set; }

    private Product() { } // EF Core

    public Product(string name, string description, Money price)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentNullException.ThrowIfNull(price);

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Price = price;
    }
}