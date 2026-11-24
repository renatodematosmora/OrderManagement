using OrderManagement.Domain.Discounts;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public DeliveryType DeliveryType { get; private set; }
    public CancelledBy? CancelledBy { get; private set; }
    public string? CancellationReason { get; private set; }

    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;
    public Money OrderAmount => _orderItems.Aggregate(Money.Zero, (total, item) => total + item.ItemAmount);

    private static readonly Dictionary<OrderStatus, HashSet<OrderStatus>> AllowedTransitions = new()
    {
        [OrderStatus.PendingApproval] = [OrderStatus.Approved, OrderStatus.Cancelled],
        [OrderStatus.Approved] = [OrderStatus.InPreparation, OrderStatus.Cancelled],
        [OrderStatus.InPreparation] = [OrderStatus.ReadyForPickup, OrderStatus.Shipped, OrderStatus.Cancelled],
        [OrderStatus.ReadyForPickup] = [OrderStatus.Delivered],
        [OrderStatus.Shipped] = [OrderStatus.Delivered],
        [OrderStatus.Delivered] = [],
        [OrderStatus.Cancelled] = [],
    };

    private Order() { } // EF Core

    public Order(Guid customerId, DeliveryType deliveryType, IEnumerable<NewOrderItem> items)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId não pode ser vazio.", nameof(customerId));

        var listItems = items?.ToList() ?? throw new ArgumentNullException(nameof(items), "A lista de itens do pedido não pode ser nula.");
        if (listItems.Count == 0)
            throw new ArgumentException("O pedido deve conter pelo menos um item.", nameof(items));

        Id = Guid.NewGuid();
        CustomerId = customerId;
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.PendingApproval;
        DeliveryType = deliveryType;

        foreach (var item in listItems)
            AddItem(item.ProductId, item.Quantity, item.UnitPrice);
    }

    public void AddItem(Guid productId, int quantity, Money unitPrice)
    {
        ArgumentNullException.ThrowIfNull(unitPrice);
        var existingItem = _orderItems.FirstOrDefault(item => item.ProductId == productId);

        if (existingItem != null)
        {
            if (existingItem.UnitPrice != unitPrice)
                throw new InvalidOperationException("Não é possível adicionar o mesmo produto com preços unitários diferentes.");

            existingItem.IncreaseQuantity(quantity);
            return;
        }

        var orderItem = new OrderItem(Id, productId, quantity, unitPrice);
        _orderItems.Add(orderItem);
    }

    private void ChangeStatus(OrderStatus newStatus)
    {
        if (!AllowedTransitions[Status].Contains(newStatus))
            throw new InvalidOperationException($"Não é possível mudar o status de {Status} para {newStatus}.");

        Status = newStatus;
    }

    public void Approve()
    {
        ChangeStatus(OrderStatus.Approved);
    }

    public void StartPreparation()
    {
        ChangeStatus(OrderStatus.InPreparation);
    }

    public void MarkReadyForPickup()
    {
        if (DeliveryType != DeliveryType.Pickup)
            throw new InvalidOperationException("Este pedido não é do tipo retirada na loja.");

        ChangeStatus(OrderStatus.ReadyForPickup);
    }

    public void Ship()
    {
        if (DeliveryType != DeliveryType.Delivery)
            throw new InvalidOperationException("Este pedido não é do tipo entrega.");

        ChangeStatus(OrderStatus.Shipped);
    }

    public void Deliver()
    {
        ChangeStatus(OrderStatus.Delivered);
    }

    public void Cancel(CancelledBy cancelledBy, string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        ChangeStatus(OrderStatus.Cancelled);
        CancelledBy = cancelledBy;
        CancellationReason = reason;
    }

    public Money CalculateAmountWithDiscount(IDiscountStrategy discountStrategy)
    {
        ArgumentNullException.ThrowIfNull(discountStrategy);

        var amount = OrderAmount;
        var discount = discountStrategy.CalculateDiscount(amount);

        return new Money(amount.Amount - discount.Amount);
    }
}