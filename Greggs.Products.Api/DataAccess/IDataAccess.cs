using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greggs.Products.Api.DataAccess;

public interface IDataAccess<T>
{
   Task<IEnumerable<T>> LatestProducts(int? pageStart, int? pageSize);
}