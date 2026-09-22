namespace OrderManagement.Domain.Enums;

public enum OrderStatus
{
    PendingApproval = 0,
    Approved = 1,
    InPreparation = 2,
    ReadyForPickup = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6
}