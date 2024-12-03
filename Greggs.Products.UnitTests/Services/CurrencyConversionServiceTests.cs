using FluentAssertions;
using Greggs.Products.Api.Services;
using Xunit;

namespace Greggs.Products.UnitTests.Services;

public class CurrencyConversionServiceTests
{
    private readonly CurrencyConversionService _currencyConversionService;

    public CurrencyConversionServiceTests()
    {
        _currencyConversionService = new CurrencyConversionService();
    }

    [Theory]
    [InlineData(1.0, 1.1)] // 1 GBP = 1.1 EUR
    [InlineData(0.5, 0.55)]
    [InlineData(2.0, 2.2)]
    public void ConvertToEuro_ReturnsCorrectConversion(decimal amount, decimal expected)
    {
        // Act
        var result = _currencyConversionService.ConvertToEuro(amount, "GBP");

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ConvertToEuro_ReturnsOriginalAmount_WhenCurrencyIsNotGbp()
    {
        // Arrange
        const decimal amount = 1.0m;

        // Act
        var result = _currencyConversionService.ConvertToEuro(amount, "USD");

        // Assert
        result.Should().Be(amount);
    }

    [Fact]
    public void ConvertToEuro_ReturnsZero_WhenAmountIsZero()
    {
        // Act
        var result = _currencyConversionService.ConvertToEuro(0, "GBP");

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void ConvertToEuro_ReturnsOriginalAmount_WhenCurrencyIsInvalid()
    {
        // Arrange
        const decimal amount = 1.0m;

        // Act
        var result = _currencyConversionService.ConvertToEuro(amount, "INVALID");

        // Assert
        result.Should().Be(amount);
    }

}
