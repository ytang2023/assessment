using Synapse.Order.Router.DataProviders.Supplier;

namespace Synapse.Order.Router.Tests.DataProviders;

public sealed class EnsureSupplierDataProvider
{
    [Fact]
    public void Should_LoadSuppliersFromEmbeddedCsv_WithCorrectParsing()
    {
        var provider = new SupplierDataProvider();

        var suppliers = provider.GetSuppliers();

        Assert.NotEmpty(suppliers);

        var sup002 = Assert.Single(suppliers, s => s.SupplierId == "SUP-002");
        Assert.Equal("DME Direct LLC", sup002.SupplierName);
        Assert.True(sup002.CanMailOrder);
        Assert.Equal(8, sup002.CustomerSatisfactionScore);
        Assert.True(sup002.ServesZip("10015")); // within 00100-99999 range

        var sup003 = Assert.Single(suppliers, s => s.SupplierId == "SUP-003");
        Assert.Null(sup003.CustomerSatisfactionScore);  // "no ratings yet" → null
        Assert.True(sup003.ServesZip("77059"));         // explicit zip in list
    }
}
