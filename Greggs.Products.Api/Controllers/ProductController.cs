using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly ProductService _productService;

    public ProductController(ILogger<ProductController> logger, ProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int? pageStart = null, int? pageSize = null)
    {
        var products = await _productService.GetAllProducts(pageStart, pageSize);
        if(products.Count() == 0)
        {
            _logger.LogInformation("Products Call was null or Empty");
            throw new InvalidOperationException("No products available.");
        }
        return Ok(products);
    }

    [HttpGet("in-euro")]
    public async Task<IActionResult> GetProductsInEuro(int? pageStart = null, int? pageSize = null)
    {
        var products = await _productService.GetProductsInEuroAsync(pageStart, pageSize);
        if (products.Count() == 0)
        {
            _logger.LogInformation("Products Call was null or Empty");
            throw new InvalidOperationException("No products available.");
        }
        return Ok(products);
    }
}