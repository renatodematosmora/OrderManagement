namespace OrderManagement.Domain.ValueObjects;

public record Document
{
    public string Value { get; }

    public Document(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (value.Length != 11 && value.Length != 14)
            throw new ArgumentException("CPF/CNPJ inválido.", nameof(value));
        Value = value;
    }
}