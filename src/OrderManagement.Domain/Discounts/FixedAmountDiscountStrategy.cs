using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Discounts;

public class FixedAmountDiscountStrategy : IDiscountStrategy
{
    private readonly Money _discountAmount;

    public FixedAmountDiscountStrategy(Money discountAmount)
    {
        ArgumentNullException.ThrowIfNull(discountAmount);

        _discountAmount = discountAmount;
    }

    public Money CalculateDiscount(Money orderAmount)
    {
        ArgumentNullException.ThrowIfNull(orderAmount);

        return new Money(Math.Min(_discountAmount.Amount, orderAmount.Amount));
    }
}