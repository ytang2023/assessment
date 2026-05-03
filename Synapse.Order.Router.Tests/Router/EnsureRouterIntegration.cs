using Microsoft.Extensions.DependencyInjection;
using Synapse.Order.Router.DataProviders.Product;
using Synapse.Order.Router.DataProviders.Supplier;
using Synapse.Order.Router.Models;
using Synapse.Order.Router.Router;

namespace Synapse.Order.Router.Tests.Router;

public sealed class EnsureRouterIntegration
{
    [Fact]
    public void Should_RouteValidOrder_WhenRouterServiceResolvedFromIoc()
    {
        var provider = new ServiceCollection()
            .AddSingleton<IProductDataProvider, ProductDataProvider>()
            .AddSingleton<ISupplierDataProvider, SupplierDataProvider>()
            .AddSingleton<RouterEngine>()
            .AddSingleton<RouterService>()
            .BuildServiceProvider();

        var routerService = provider.GetRequiredService<RouterService>();

        var order = new OrderRequest
        {
            OrderId = "ORD-IOC-TEST",
            CustomerZip = "10015",
            MailOrder = false,
            Items =
            [
                new OrderItem { ProductCode = "WC-STD-001", Quantity = 1 },
                new OrderItem { ProductCode = "OX-PORT-024", Quantity = 1 }
            ]
        };

        var result = routerService.Route(order);

        Assert.True(result.Feasible);
        Assert.NotNull(result.Routing);
        Assert.NotEmpty(result.Routing!);

        var routedCodes = result.Routing!.SelectMany(a => a.Items).Select(i => i.ProductCode).ToHashSet();
        Assert.Contains("WC-STD-001", routedCodes);
        Assert.Contains("OX-PORT-024", routedCodes);
    }
}
