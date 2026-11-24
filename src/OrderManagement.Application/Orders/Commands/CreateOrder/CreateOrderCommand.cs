using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(
    Guid CustomerId,
    DeliveryType DeliveryType,
    IReadOnlyList<CreateOrderItemInput> Items);

public record CreateOrderItemInput(Guid ProductId, int Quantity);