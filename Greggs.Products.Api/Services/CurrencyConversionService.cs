using Greggs.Products.Api.Interfaces;

namespace Greggs.Products.Api.Services;

public class CurrencyConversionService : ICurrencyConversionService
{
    private const decimal GBPToEurosConversion = 1.1m;

    public decimal ConvertToEuro(decimal amount, string currency)
    {
        return currency.ToUpper() == "GBP" ? amount * GBPToEurosConversion : amount;
    }
}
