using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Entities;

public readonly record struct NewOrderItem(Guid ProductId, int Quantity, Money UnitPrice);