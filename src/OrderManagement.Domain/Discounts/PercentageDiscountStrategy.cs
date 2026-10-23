using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Discounts;

public class PercentageDiscountStrategy : IDiscountStrategy
{
    private readonly decimal _percentage;

    public PercentageDiscountStrategy(decimal percentage)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(percentage);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(percentage, 1m);

        _percentage = percentage;
    }

    public Money CalculateDiscount(Money orderAmount)
    {
        ArgumentNullException.ThrowIfNull(orderAmount);

        var discount = Math.Round(orderAmount.Amount * _percentage, 2, MidpointRounding.AwayFromZero);
        return new Money(discount);
    }
}