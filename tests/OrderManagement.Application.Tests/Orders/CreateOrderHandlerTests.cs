using OrderManagement.Application.Exceptions;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using OrderManagement.Application.Tests.Fakes;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.ValueObjects;

using Xunit;

namespace OrderManagement.Application.Tests.Orders;

public class CreateOrderHandlerTests
{
    private readonly FakeOrderRepository _orders = new();
    private readonly FakeCustomerRepository _customers = new();
    private readonly FakeProductRepository _products = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly CreateOrderHandler _handler;

    public CreateOrderHandlerTests()
    {
        _handler = new CreateOrderHandler(_orders, _customers, _products, _unitOfWork);
    }

    private async Task<(Customer Customer, Product Product)> CadastrarClienteEProdutoAsync()
    {
        var customer = new Customer("Maria Silva", new Document("12345678901"));
        var product = new Product("Pizza", "Pizza grande", new Money(40m));
        await _customers.AddAsync(customer);
        await _products.AddAsync(product);
        return (customer, product);
    }

    [Fact]
    public async Task Deve_criar_pedido_e_persistir_quando_os_dados_sao_validos()
    {
        // Arrange
        var (customer, product) = await CadastrarClienteEProdutoAsync();

        var command = new CreateOrderCommand(
            customer.Id,
            DeliveryType.Delivery,
            [new CreateOrderItemInput(product.Id, 2)]);

        // Act
        var orderId = await _handler.HandleAsync(command);

        // Assert
        Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
        var order = await _orders.GetByIdAsync(orderId);
        Assert.NotNull(order);
        Assert.Equal(new Money(80m), order.OrderAmount);
    }

    [Fact]
    public async Task Nao_deve_criar_pedido_quando_o_cliente_nao_existe()
    {
        // Arrange
        var (customer, product) = await CadastrarClienteEProdutoAsync();

        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            DeliveryType.Delivery,
            [new CreateOrderItemInput(product.Id, 1)]);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.HandleAsync(command));
        Assert.Equal(0, _unitOfWork.SaveChangesCallCount);
        Assert.Empty(await _orders.GetAllAsync());
    }

    [Fact]
    public async Task Nao_deve_criar_pedido_quando_o_produto_nao_existe()
    {
        // Arrange
        var (customer, product) = await CadastrarClienteEProdutoAsync();
        var produtoInexistente = Guid.NewGuid();

        var command = new CreateOrderCommand(
            customer.Id,
            DeliveryType.Delivery,
            [new CreateOrderItemInput(produtoInexistente, 1)]);

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _handler.HandleAsync(command));

        // Assert
        Assert.Contains(produtoInexistente.ToString(), exception.Message);
        Assert.Equal(0, _unitOfWork.SaveChangesCallCount);
        Assert.Empty(await _orders.GetAllAsync());
    }

    [Fact]
    public async Task Nao_deve_criar_pedido_com_lista_de_itens_vazia()
    {
        // Arrange
        var (customer, product) = await CadastrarClienteEProdutoAsync();

        var command = new CreateOrderCommand(
            customer.Id,
            DeliveryType.Delivery,
            []);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.HandleAsync(command));
        Assert.Equal(0, _unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Deve_somar_quantidades_quando_o_mesmo_produto_aparece_duas_vezes_no_comando()
    {
        // Arrange
        var (customer, product) = await CadastrarClienteEProdutoAsync();
        var command = new CreateOrderCommand(
            customer.Id,
            DeliveryType.Delivery,
            [
                new CreateOrderItemInput(product.Id, 2),
                new CreateOrderItemInput(product.Id, 3)
            ]);

        // Act
        var orderId = await _handler.HandleAsync(command);

        // Assert
        var order = await _orders.GetByIdAsync(orderId);
        Assert.NotNull(order);
        var item = Assert.Single(order.OrderItems);
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public async Task Deve_usar_o_preco_do_produto_como_preco_unitario_do_item()
    {
        // Arrange
        var (customer, product) = await CadastrarClienteEProdutoAsync();
        var command = new CreateOrderCommand(
            customer.Id,
            DeliveryType.Delivery,
            [new CreateOrderItemInput(product.Id, 1)]);

        // Act
        var orderId = await _handler.HandleAsync(command);

        // Assert
        var order = await _orders.GetByIdAsync(orderId);
        Assert.NotNull(order);
        var item = Assert.Single(order.OrderItems);
        Assert.Equal(product.Price, item.UnitPrice);
    }
}