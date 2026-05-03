using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using Synapse.Order.Router.Models;

namespace Synapse.Order.Router.API;

/// <summary>
/// Provides sample request bodies shown in the Swagger UI for the POST /api/route endpoint.
/// </summary>
public sealed class RouteOrderRequestExample : OpenApiExample<OrderRequest>
{
    public override IOpenApiExample<OrderRequest> Build(NamingStrategy? namingStrategy = null)
    {
        // ORD-001 — local delivery: wheelchair + portable oxygen, NYC zip
        Examples.Add(OpenApiExampleResolver.Resolve(
            "LocalDelivery",
            new OrderRequest
            {
                OrderId = "ORD-001",
                CustomerZip = "10015",
                MailOrder = false,
                Items =
                [
                    new OrderItem { ProductCode = "WC-STD-001", Quantity = 1 },
                    new OrderItem { ProductCode = "OX-PORT-024", Quantity = 1 }
                ]
            },
            new SnakeCaseNamingStrategy()));

        // ORD-002 — local delivery: multi-item order, Houston zip
        Examples.Add(OpenApiExampleResolver.Resolve(
            "MultiItemLocalDelivery",
            new OrderRequest
            {
                OrderId = "ORD-002",
                CustomerZip = "77059",
                MailOrder = false,
                Items =
                [
                    new OrderItem { ProductCode = "HB-FUL-018", Quantity = 1 },
                    new OrderItem { ProductCode = "PL-ELEC-043", Quantity = 1 },
                    new OrderItem { ProductCode = "CM-BED-048", Quantity = 1 },
                    new OrderItem { ProductCode = "BP-AUTO-077", Quantity = 1 }
                ]
            },
            new SnakeCaseNamingStrategy()));

        // ORD-003 — mail order: CPAP supplies, Boston zip
        Examples.Add(OpenApiExampleResolver.Resolve(
            "MailOrder",
            new OrderRequest
            {
                OrderId = "ORD-003",
                CustomerZip = "02130",
                MailOrder = true,
                Items =
                [
                    new OrderItem { ProductCode = "CP-STD-031", Quantity = 1 },
                    new OrderItem { ProductCode = "CP-MSK-FF-035", Quantity = 2 },
                    new OrderItem { ProductCode = "NB-COMP-039", Quantity = 1 }
                ]
            },
            new SnakeCaseNamingStrategy()));

        return this;
    }
}
