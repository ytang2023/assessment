using Models = Synapse.Order.Router.Models;

namespace Synapse.Order.Router.DataProviders.Supplier;

public interface ISupplierDataProvider
{
    IReadOnlyList<Models.Supplier> GetSuppliers();
}
