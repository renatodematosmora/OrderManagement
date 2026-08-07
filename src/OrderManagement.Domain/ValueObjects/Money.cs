namespace OrderManagement.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; }

    public Money(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        Amount = amount;
    }

    public static Money Zero => new(0);

    public static Money operator +(Money a, Money b) => new(a.Amount + b.Amount);
    public static Money operator *(Money money, int quantity) => new(money.Amount * quantity);

    public override string ToString() => Amount.ToString("C");
}