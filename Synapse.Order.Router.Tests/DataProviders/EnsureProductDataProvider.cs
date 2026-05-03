using Synapse.Order.Router.DataProviders.Product;

namespace Synapse.Order.Router.Tests.DataProviders;

public sealed class EnsureProductDataProvider
{
    [Fact]
    public void Should_LoadProductsFromEmbeddedCsv_WithCorrectMapping()
    {
        var provider = new ProductDataProvider();

        var products = provider.GetProducts();

        Assert.NotEmpty(products);
        var product = Assert.Contains("WC-STD-001", products);
        Assert.Equal("Standard Wheelchair", product.ProductName);
        Assert.Equal("wheelchair", product.Category);
    }
}
