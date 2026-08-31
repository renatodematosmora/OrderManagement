using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);
}