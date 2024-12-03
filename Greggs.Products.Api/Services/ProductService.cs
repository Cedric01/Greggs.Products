using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Interfaces;
using Greggs.Products.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greggs.Products.Api.Services;

public class ProductService
{
    private readonly ICurrencyConversionService _currencyConversionService;

    private readonly IDataAccess<Product> _dataAccess;
    public ProductService(IDataAccess<Product> dataAccess, ICurrencyConversionService currencyConversionService)
    {
        _dataAccess = dataAccess;
        _currencyConversionService = currencyConversionService;
    }

    public async Task <IEnumerable<Product>> GetAllProducts(int? pageStart = null, int? pageSize = null)
    {
        return await _dataAccess.LatestProducts(pageStart, pageSize);
    }

    public async Task<IEnumerable<Product>> GetProductsInEuroAsync(int? pageStart = null, int? pageSize = null)
    {
        var products = await _dataAccess.LatestProducts(pageStart, pageSize);
        foreach (var product in products)
        {
            product.PriceInPounds = _currencyConversionService.ConvertToEuro(product.PriceInPounds, "GBP");
        }
        return products;
    }
}
