using OrderManagement.Domain.Discounts;
using OrderManagement.Domain.ValueObjects;

namespace OrderManagement.Domain.Tests;

public class DiscountStrategyTests
{
    [Fact]
    public void Percentual_deve_calcular_dez_por_cento_do_valor_do_pedido()
    {
        // Arrange
        var strategy = new PercentageDiscountStrategy(0.1m);

        // Act
        var discount = strategy.CalculateDiscount(new Money(200));

        // Assert
        Assert.Equal(new Money(20), discount);
    }

    [Fact]
    public void Percentual_deve_calcular_arredondamento_de_casas_decimais()
    {
        // Arrange
        var strategy = new PercentageDiscountStrategy(0.1m);

        // Act
        var discount = strategy.CalculateDiscount(new Money(33.33m));

        // Assert
        Assert.Equal(new Money(3.33m), discount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.1)]
    [InlineData(-1)]
    public void Percentual_zero_ou_negativo_deve_lancar_excecao(double percentual)
    {
        // Arrange Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new PercentageDiscountStrategy((decimal)percentual));
    }

    [Theory]
    [InlineData(1.1)]
    [InlineData(1.5)]
    public void Percentual_maior_que_cem_porcento_deve_lancar_excecao(double percentual)
    {
        // Arrange Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new PercentageDiscountStrategy((decimal)percentual));
    }

    [Fact]
    public void Percentual_cem_por_cento_deve_calcular_o_valor_total_do_pedido()
    {
        // Arrange
        var strategy = new PercentageDiscountStrategy(1.0m);

        // Act
        var discount = strategy.CalculateDiscount(new Money(200));

        // Assert
        Assert.Equal(new Money(200), discount);
    }

    [Fact]
    public void Desconto_fixo_menor_que_pedido()
    {
        // Arrange
        var strategy = new FixedAmountDiscountStrategy(new Money(10m));

        // Act 
        var discount = strategy.CalculateDiscount(new Money(30m));
        
        // Assert
        Assert.Equal(new Money(10m), discount);
    }

    [Fact]
    public void Desconto_fixo_maior_que_pedido_limitado_ao_total_do_pedido()
    {
        // Arrange
        var strategy = new FixedAmountDiscountStrategy(new Money(40m));

        // Act 
        var discount = strategy.CalculateDiscount(new Money(30m));
        
        // Assert
        Assert.Equal(new Money(30m), discount);
    }
}