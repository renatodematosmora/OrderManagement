using OrderManagement.Domain.Enums;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;
    public Money OrderAmount => _orderItems.Aggregate(Money.Zero, (total, item) => total + item.ItemAmount);

    private Order() { } // EF Core

    public Order(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId não pode ser vazio.", nameof(customerId));

        Id = Guid.NewGuid();
        CustomerId = customerId;
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Pending;
    }

    public void AddItem(Guid productId, int quantity, Money unitPrice)
    {
        var orderItem = new OrderItem(Id, productId, quantity, unitPrice);
        _orderItems.Add(orderItem);
    }
}