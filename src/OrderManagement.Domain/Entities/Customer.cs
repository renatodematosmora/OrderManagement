using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; }
    public Document CpfCnpj { get; private set; }

    public Customer(string fullName, Document cpfCnpj)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentNullException.ThrowIfNull(cpfCnpj);
        Id = Guid.NewGuid();
        FullName = fullName;
        CpfCnpj = cpfCnpj;
    }
}