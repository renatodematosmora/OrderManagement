namespace OrderManagement.Domain.Entities;

public class Customer
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; }
    public string Document { get; private set; }

    public Customer(string fullName, string document)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(document);

        Id = Guid.NewGuid();
        FullName = fullName;
        Document = document;
    }
}