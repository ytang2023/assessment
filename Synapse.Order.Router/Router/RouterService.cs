using Synapse.Order.Router.DataProviders.Product;
using Synapse.Order.Router.DataProviders.Supplier;
using Synapse.Order.Router.Models;

namespace Synapse.Order.Router.Router;

public sealed class RouterService
{
    private readonly IProductDataProvider _products;
    private readonly ISupplierDataProvider _suppliers;
    private readonly RouterEngine _engine;

    public RouterService(
        IProductDataProvider products,
        ISupplierDataProvider suppliers,
        RouterEngine engine)
    {
        _products = products;
        _suppliers = suppliers;
        _engine = engine;
    }

    public RoutingResult Route(OrderRequest order)
    {
        try
        {
            return _engine.Route(order, _suppliers.GetSuppliers(), _products.GetProducts());
        }
        catch (Exception ex)
        {
            return new RoutingResult
            {
                Feasible = false,
                Errors = [$"An unexpected error occurred: {ex.Message}"]
            };
        }
    }
}
