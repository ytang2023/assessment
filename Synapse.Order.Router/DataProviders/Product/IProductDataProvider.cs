using Models = Synapse.Order.Router.Models;

namespace Synapse.Order.Router.DataProviders.Product;

public interface IProductDataProvider
{
    IReadOnlyDictionary<string, Models.Product> GetProducts();
}
