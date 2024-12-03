using Castle.Core.Logging;
using FluentAssertions;
using Greggs.Products.Api.Controllers;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Interfaces;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;

namespace Greggs.Products.UnitTests.Controllers;

public class ProductControllerTests
{
    private Mock<ICurrencyConversionService> _mockCurrencyConversionService;
    private Mock<ILogger<ProductController>> _LoggerMock;
    private readonly Mock<IDataAccess<Product>> _mockDataAccess;
    private const string ERRORMESSAGE = "Products Call was null or Empty";
    private const string INVALIDOPERATIONMESSAGE = "No products available.";

    private ProductController _controller;
    private readonly ProductService _productService;
    public ProductControllerTests()
    {
        _mockDataAccess = new Mock<IDataAccess<Product>>();
        _mockCurrencyConversionService = new Mock<ICurrencyConversionService>();
        _LoggerMock = new Mock<ILogger<ProductController>>();

        _productService = new ProductService(_mockDataAccess.Object, _mockCurrencyConversionService.Object);
        _controller = new ProductController(_LoggerMock.Object, _productService);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsOkResult_WithListOfProducts()
    {
        // Arrange
        var products = new List<Product>
            {
                new Product { Name = "TestOne", PriceInPounds = 1.1m, LastUpdated = DateTime.UtcNow },
                new Product { Name = "TestTwo", PriceInPounds = 2.2m, LastUpdated = DateTime.UtcNow.AddDays(-1) }
            };

        _mockDataAccess.Setup(x => x.LatestProducts(null, null)).ReturnsAsync(products);

        // Act
        var result = await _controller.GetAll(null, null);

        // Assert
        result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(200);
        var okResult = result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(products);
    }

    [Fact]
    public async Task GetProductsInEuro_ReturnsOkResult_WithConvertedPrices()
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

        _mockDataAccess.Setup(x => x.LatestProducts(null, null)).ReturnsAsync(products);

        const decimal conversionRate = 1.1m;
        _mockCurrencyConversionService
            .Setup(x => x.ConvertToEuro(It.IsAny<decimal>(), "GBP"))
            .Returns<decimal, string>((amount, _) => amount * conversionRate);

        // Act
        var result = await _controller.GetProductsInEuro(null, null);

        // Assert
        result.Should().BeOfType<OkObjectResult>().Which.StatusCode.Should().Be(200);

        var okResult = result as OkObjectResult;
        var convertedProducts = okResult.Value as IEnumerable<Product>;
        convertedProducts.Should().NotBeNull();

        foreach (var product in convertedProducts)
        {
            var original = originalProducts.Find(p => p.Name == product.Name);
            product.PriceInPounds.Should().Be(original.PriceInPounds * conversionRate);
        }
    }

    [Fact]
    public async Task GetProducts_LogsInformation_WhenInvoked()
    {
        // Arrange

        _mockDataAccess.Setup(x => x.LatestProducts(null, null)).ReturnsAsync(new List<Product>());

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetAll());
        exception.Message.Should().Be(INVALIDOPERATIONMESSAGE);

        // Assert
        _LoggerMock.Verify(
            log => log.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(ERRORMESSAGE)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetProducts_InEuros_LogsInformation_WhenInvoked()
    {
        // Arrange
        _mockDataAccess.Setup(x => x.LatestProducts(null, null)).ReturnsAsync(new List<Product>());

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetProductsInEuro());
        exception.Message.Should().Be(INVALIDOPERATIONMESSAGE);

        // Assert
        _LoggerMock.Verify(
            log => log.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(ERRORMESSAGE)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task GetProducts_ThrowsException_WhenProductsListIsEmpty()
    {
        // Act
        _mockDataAccess.Setup(x => x.LatestProducts(null, null)).ReturnsAsync(new List<Product>());

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetAll());
        exception.Message.Should().Be(INVALIDOPERATIONMESSAGE);
    }

    [Fact]
    public async Task GetProducts_InEuros_ThrowsException_WhenProductsListIsEmpty()
    {
        // Act
        _mockDataAccess.Setup(x => x.LatestProducts(null, null)).ReturnsAsync(new List<Product>());

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetProductsInEuro());
        exception.Message.Should().Be(INVALIDOPERATIONMESSAGE);
    }

}