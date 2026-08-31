using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Contexts;

namespace OrderManagement.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
    }

    public async Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.OrderItems)
            .ToListAsync(cancellationToken);
    }
}