using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Synapse.Order.Router.DataProviders.Product;
using Synapse.Order.Router.DataProviders.Supplier;
using Synapse.Order.Router.Router;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddSingleton<IOpenApiConfigurationOptions>(_ => new OpenApiConfigurationOptions
{
    Info = new OpenApiInfo
    {
        Version = "1.0.0",
        Title = "Synapse Order Router API",
        Description = "Routes DME/HME orders to one or more suppliers based on product category, " +
                      "ZIP-range service areas, mail-order capability, and customer satisfaction score."
    },
    OpenApiVersion = OpenApiVersionType.V3
});

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton<IProductDataProvider, ProductDataProvider>();
builder.Services.AddSingleton<ISupplierDataProvider, SupplierDataProvider>();
builder.Services.AddSingleton<RouterEngine>();
builder.Services.AddSingleton<RouterService>();

builder.Build().Run();

