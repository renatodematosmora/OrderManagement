using OrderManagement.Domain.Discounts;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.ValueObjects;

using Xunit;

namespace OrderManagement.Domain.Tests;

public class OrderTests
{
    // Helpers
    private static List<NewOrderItem> ItensValidos() =>
        [new NewOrderItem(Guid.NewGuid(), 1, new Money(10m))];

    private static Order CriarPedidoValido(DeliveryType deliveryType = DeliveryType.Delivery) =>
        new(Guid.NewGuid(), deliveryType, ItensValidos());

    // Criação
    [Fact]
    public void Deve_criar_pedido_com_status_pending_approval()
    {
        var order = CriarPedidoValido();

        // Assert
        Assert.Equal(OrderStatus.PendingApproval, order.Status);
    }

    [Fact]
    public void Nao_deve_permitir_criar_pedido_com_customerId_vazio()
    {
        Assert.Throws<ArgumentException>(
            () => new Order(Guid.Empty, DeliveryType.Delivery, ItensValidos()));
    }

    [Fact]
    public void Nao_deve_permitir_criar_pedido_com_lista_de_itens_vazia()
    {
        Assert.Throws<ArgumentException>(
            () => new Order(Guid.NewGuid(), DeliveryType.Delivery, new List<NewOrderItem>()));
    }

    [Fact]
    public void Nao_deve_permitir_criar_pedido_com_lista_de_itens_nula()
    {
        Assert.Throws<ArgumentNullException>(
            () => new Order(Guid.NewGuid(), DeliveryType.Delivery, null!));
    }

    // Itens duplicados
    [Fact]
    public void Deve_somar_quantidades_quando_o_mesmo_produto_aparece_duas_vezes()
    {
        var productId = Guid.NewGuid();
        var itens = new List<NewOrderItem>
        {
            new(productId, 2, new Money(10m)),
            new(productId, 3, new Money(10m))
        };

        var order = new Order(Guid.NewGuid(), DeliveryType.Delivery, itens);

        var item = Assert.Single(order.OrderItems);
        Assert.Equal(5, item.Quantity);
        Assert.Equal(new Money(50m), order.OrderAmount);
    }

    [Fact]
    public void Nao_deve_permitir_mesmo_produto_com_precos_diferentes()
    {
        var productId = Guid.NewGuid();
        var itens = new List<NewOrderItem>
        {
            new(productId, 1, new Money(10m)),
            new(productId, 1, new Money(12m))
        };

        Assert.Throws<InvalidOperationException>(
            () => new Order(Guid.NewGuid(), DeliveryType.Delivery, itens));
    }

    [Fact]
    public void Deve_atualizar_pedido_com_status_approved()
    {
        // Arrange
        var order = CriarPedidoValido();

        // Act
        order.Approve();

        // Assert
        Assert.Equal(OrderStatus.Approved, order.Status);
    }

    [Fact]
    public void Nao_deve_permitir_atualizar_pedido_para_status_invalido()
    {
        // Arrange
        var order = CriarPedidoValido();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Ship());
    }

    [Fact]
    public void Nao_deve_permitir_pedido_de_retirada_ser_enviado()
    {
        // Arrange
        var order = CriarPedidoValido(DeliveryType.Pickup);
        order.Approve();
        order.StartPreparation();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Ship());
    }

    [Fact]
    public void Deve_preencher_CancelledBy_e_CancellationReason_ao_cancelar()
    {
        // Arrange
        var order = CriarPedidoValido();

        // Act
        order.Cancel(CancelledBy.Customer, "Cliente desistiu da compra");

        // Assert
        Assert.Equal(OrderStatus.Cancelled, order.Status);
        Assert.Equal(CancelledBy.Customer, order.CancelledBy);
        Assert.Equal("Cliente desistiu da compra", order.CancellationReason);
    }

    [Fact]
    public void Aplica_desconto_percentual_ao_pedido_com_item()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), DeliveryType.Delivery,
            [new NewOrderItem(Guid.NewGuid(), 2, new Money(50m))]);

        // Act
        var amountWithDiscount = order.CalculateAmountWithDiscount(new PercentageDiscountStrategy(0.1m));

        // Assert
        Assert.Equal(new Money(90m), amountWithDiscount);
    }

    [Fact]
    public void Aplica_desconto_fixo_ao_pedido_com_item()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), DeliveryType.Delivery,
            [new NewOrderItem(Guid.NewGuid(), 2, new Money(50m))]);

        // Act
        var amountWithDiscount = order.CalculateAmountWithDiscount(new FixedAmountDiscountStrategy(new Money(10m)));

        // Assert
        Assert.Equal(new Money(90m), amountWithDiscount);
    }

    [Fact]
    public void Deve_somar_quantidade_ao_adicionar_item_de_produto_ja_existente()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var order = new Order(Guid.NewGuid(), DeliveryType.Delivery,
            [new NewOrderItem(productId, 2, new Money(50m))]);

        // Act
        order.AddItem(productId, 3, new Money(50m));

        // Assert
        var item = Assert.Single(order.OrderItems);
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void Nao_deve_permitir_adicionar_item_com_quantidade_zero()
    {
        // Arrange
        var order = CriarPedidoValido();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => order.AddItem(Guid.NewGuid(), 0, new Money(10m)));
    }
}