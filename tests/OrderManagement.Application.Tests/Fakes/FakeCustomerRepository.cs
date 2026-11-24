using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Tests.Fakes;

public class FakeCustomerRepository : ICustomerRepository
{
    private readonly List<Customer> _customers = new();

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_customers.FirstOrDefault(c => c.Id == id));

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        _customers.Add(customer);
        return Task.CompletedTask;
    }
}