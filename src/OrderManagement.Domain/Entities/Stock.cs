namespace OrderManagement.Domain.Entities;

public class Stock
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }

    public Stock(Guid productId, int quantity)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId não pode ser vazio.", nameof(productId));
        ArgumentOutOfRangeException.ThrowIfNegative(quantity);

        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
    }
}