using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Synapse.Order.Router.Models;
using Synapse.Order.Router.Router;
using System.Net;

namespace Synapse.Order.Router.API;

public sealed class RouterController
{
    private readonly ILogger<RouterController> _logger;
    private readonly RouterService _routerService;

    public RouterController(ILogger<RouterController> logger, RouterService routerService)
    {
        _logger = logger;
        _routerService = routerService;
    }

    [Function(nameof(RouteOrder))]
    [OpenApiOperation(
        operationId: nameof(RouteOrder),
        tags: ["Orders"],
        Summary = "Route an order to one or more suppliers.",
        Description = "Accepts an order with customer ZIP, mail-order flag, and line items. " +
                      "Returns a feasibility flag and per-supplier assignments based on product category coverage, " +
                      "service ZIP ranges, and customer satisfaction scores.")]
    [OpenApiRequestBody(
        contentType: "application/json",
        bodyType: typeof(OrderRequest),
        Description = "Order to route. Provide customer_zip OR set mail_order=true for nationwide fulfillment.",
        Required = true,
        Example = typeof(RouteOrderRequestExample))]
    [OpenApiResponseWithBody(
        statusCode: HttpStatusCode.OK,
        contentType: "application/json",
        bodyType: typeof(RoutingResult),
        Summary = "Routing result.",
        Description = "When feasible=true the routing array lists each supplier and the items it will fulfil. " +
                      "When feasible=false the errors array explains why the order cannot be routed.")]
    [OpenApiResponseWithBody(
        statusCode: HttpStatusCode.BadRequest,
        contentType: "application/json",
        bodyType: typeof(RoutingResult),
        Summary = "Invalid or empty request body.")]
    public async Task<HttpResponseData> RouteOrder(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "route")] HttpRequestData req)
    {
        _logger.LogInformation("Processing order route request.");

        var order = await req.ReadFromJsonAsync<OrderRequest>();

        if (order is null)
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteAsJsonAsync(new RoutingResult
            {
                Feasible = false,
                Errors = ["Invalid or empty request body."]
            });
            return bad;
        }

        var result = _routerService.Route(order);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(result);
        return response;
    }
}
