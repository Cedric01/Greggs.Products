namespace Greggs.Products.Api.Interfaces;

public interface ICurrencyConversionService
{
    decimal ConvertToEuro(decimal amount, string currency);
}
