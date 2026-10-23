using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Discounts;

public interface IDiscountStrategy
{
    Money CalculateDiscount(Money orderAmount);
}