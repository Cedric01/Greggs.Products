using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Greggs.Products.Api.Models;

namespace Greggs.Products.Api.DataAccess;

/// <summary>
/// DISCLAIMER: This is only here to help enable the purpose of this exercise, this doesn't reflect the way we work!
/// </summary>
public class ProductAccess : IDataAccess<Product>
{
    private static readonly IEnumerable<Product> ProductDatabase = new List<Product>()
    {
        new() { Name = "Sausage Roll", PriceInPounds = 1m, LastUpdated = DateTime.UtcNow.AddDays(-1) },
        new() { Name = "Vegan Sausage Roll", PriceInPounds = 1.1m, LastUpdated = DateTime.UtcNow.AddDays(-2) },
        new() { Name = "Steak Bake", PriceInPounds = 1.2m, LastUpdated = DateTime.UtcNow.AddDays(-3) },
        new() { Name = "Yum Yum", PriceInPounds = 0.7m, LastUpdated = DateTime.UtcNow.AddDays(-4) },
        new() { Name = "Pink Jammie", PriceInPounds = 0.5m, LastUpdated = DateTime.UtcNow.AddDays(-5) },
        new() { Name = "Mexican Baguette", PriceInPounds = 2.1m, LastUpdated = DateTime.UtcNow.AddDays(-6) },
        new() { Name = "Bacon Sandwich", PriceInPounds = 1.95m, LastUpdated = DateTime.UtcNow.AddDays(-7) },
        new() { Name = "Coca Cola", PriceInPounds = 1.2m, LastUpdated = DateTime.UtcNow.AddDays(-8) }
    };

    public Task<IEnumerable<Product>> LatestProducts(int? pageStart, int? pageSize)
    {
        return Task.Run(() =>
        {
            var queryable = ProductDatabase.AsQueryable()
                .Where(x => x.LastUpdated >= DateTime.UtcNow.AddDays(-7));


            if (pageStart.HasValue)
                queryable = queryable.Skip(pageStart.Value);

            if (pageSize.HasValue)
                queryable = queryable.Take(pageSize.Value);

            return queryable.ToList().AsEnumerable();
        });
    }
}