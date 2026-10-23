using OrderManagement.Domain.Discounts;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.ValueObjects;

using Xunit;

namespace OrderManagement.Domain.Tests;

public class OrderTests
{
    [Fact]
    public void Deve_criar_pedido_com_status_pending_approval()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var order = new Order(customerId, DeliveryType.Delivery);

        // Assert
        Assert.Equal(OrderStatus.PendingApproval, order.Status);
    }

    [Fact]
    public void Nao_deve_permitir_criar_pedido_com_customerId_vazio()
    {
        Assert.Throws<ArgumentException>(() => new Order(Guid.Empty, DeliveryType.Delivery));
    }

    [Fact]
    public void Deve_atualizar_pedido_com_status_approved()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var order = new Order(customerId, DeliveryType.Delivery);

        // Act
        order.Approve();

        // Assert
        Assert.Equal(OrderStatus.Approved, order.Status);
    }

    [Fact]
    public void Nao_deve_permitir_atualizar_pedido_para_status_invalido()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var order = new Order(customerId, DeliveryType.Delivery);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Ship());
    }

    [Fact]
    public void Nao_deve_permitir_pedido_de_retirada_ser_enviado()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var order = new Order(customerId, DeliveryType.Pickup);
        order.Approve();
        order.StartPreparation();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Ship());
    }

    [Fact]
    public void Deve_preencher_CancelledBy_e_CancellationReason_ao_cancelar()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), DeliveryType.Delivery);

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
        var order = new Order(Guid.NewGuid(), DeliveryType.Delivery);
        order.AddItem(Guid.NewGuid(), 2, new Money(50m));

        // Act
        var amountWithDiscount = order.CalculateAmountWithDiscount(new PercentageDiscountStrategy(0.1m));

        // Assert
        Assert.Equal(new Money(90m), amountWithDiscount);
    }

    [Fact]
    public void Aplica_desconto_fixo_ao_pedido_com_item()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), DeliveryType.Delivery);
        order.AddItem(Guid.NewGuid(), 2, new Money(50m));

        // Act
        var amountWithDiscount = order.CalculateAmountWithDiscount(new FixedAmountDiscountStrategy(new Money(10m)));

        // Assert
        Assert.Equal(new Money(90m), amountWithDiscount);
    }
}