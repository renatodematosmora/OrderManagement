using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Tests.Fakes;

public class FakeProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();

    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _products.Add(product);
        return Task.CompletedTask;
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

    public Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        => Task.FromResult(_products.Where(p => ids.Contains(p.Id)).ToList());
}