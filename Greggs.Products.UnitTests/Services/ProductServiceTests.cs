

using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Interfaces;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace Greggs.Products.UnitTests.Services;

public class ProductServiceTests
{
    private readonly Mock<IDataAccess<Product>> _mockDataAccess;
    private readonly Mock<ICurrencyConversionService> _mockCurrencyConversionService;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockDataAccess = new Mock<IDataAccess<Product>>();
        _mockCurrencyConversionService = new Mock<ICurrencyConversionService>();
        _productService = new ProductService(_mockDataAccess.Object, _mockCurrencyConversionService.Object);
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<Product>
            {
                new Product { Name = "TestOne", PriceInPounds = 1.0m, LastUpdated = DateTime.UtcNow },
                new Product { Name = "TestTwo", PriceInPounds = 2.0m, LastUpdated = DateTime.UtcNow.AddDays(-1) }
            };
        _mockDataAccess.Setup(x => x.LatestProducts(null, null)).ReturnsAsync(products);

        // Act
        var result = await _productService.GetAllProducts(null, null);

        // Assert
        result.Should().BeEquivalentTo(products);
    }

    [Fact]
    public async Task GetProductsInEuroAsync_ReturnsConvertedPrices()
    {
        // Arrange
        var products = new List<Product>
            {
                new Product { Name = "TestOne", PriceInPounds = 1.0m, LastUpdated = DateTime.UtcNow },
                new Product { Name = "TestTwo", PriceInPounds = 2.0m, LastUpdated = DateTime.UtcNow.AddDays(-1) }
            };

        var originalProducts = new List<Product>
            {
                new Product { Name = "TestOne", PriceInPounds = 1.0m, LastUpdated = DateTime.UtcNow },
                new Product { Name = "TestTwo", PriceInPounds = 2.0m, LastUpdated = DateTime.UtcNow.AddDays(-1) }
            };


        _mockDataAccess.Setup(da => da.LatestProducts(null, null)).ReturnsAsync(products);

        const decimal conversionRate = 1.1m;
        _mockCurrencyConversionService
            .Setup(x => x.ConvertToEuro(It.IsAny<decimal>(), "GBP"))
            .Returns<decimal, string>((amount, _) => amount * conversionRate);

        // Act
        var result = await _productService.GetProductsInEuroAsync(null, null);

        // Assert
        foreach (var product in result)
        {
            var original = originalProducts.Find(p => p.Name == product.Name);
            product.PriceInPounds.Should().Be(original.PriceInPounds * conversionRate);
        }
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsEmptyList_WhenNoProductsExist()
    {
        // Arrange
        _mockDataAccess.Setup(x => x.LatestProducts(null, null)).ReturnsAsync(new List<Product>());

        // Act
        var result = await _productService.GetAllProducts(null, null);

        // Assert
        result.Should().BeEmpty();
    }
}
