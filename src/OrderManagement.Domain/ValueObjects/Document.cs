namespace OrderManagement.Domain.ValueObjects;

public record Document
{
    public string CpfCnpj { get; }

    public Document(string cpfCnpj)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cpfCnpj);
        if (cpfCnpj.Length != 11 && cpfCnpj.Length != 14)
            throw new ArgumentException("CPF/CNPJ inválido.", nameof(cpfCnpj));
        CpfCnpj = cpfCnpj;
    }
}