using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Interfaces.Messaging;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Commands.CreateOrder;

public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderHandler(IOrderRepository orderRepository,
                              ICustomerRepository customerRepository,
                              IProductRepository productRepository,
                              IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer == null)
            throw new NotFoundException("Customer", command.CustomerId);

        var productIds = command.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = (await _productRepository.GetByIdsAsync(productIds, cancellationToken)).ToDictionary(p => p.Id);
        var missingIds = productIds.Where(id => !products.ContainsKey(id)).ToList();
        if (missingIds.Count > 0)
            throw new NotFoundException("Product", Guid.Parse(string.Join(",", missingIds)));

        var order = new Order(
                            command.CustomerId, command.DeliveryType, command.Items.Select(
                                i => new NewOrderItem(
                                    i.ProductId, i.Quantity, products[i.ProductId].Price
                                )
                            )
                        );
        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return order.Id;
    }
}