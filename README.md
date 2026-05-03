# Synapse Order Router

An Azure Functions (isolated worker, .NET 8) HTTP service that accepts an incoming order and routes it to the optimal supplier(s). The routing algorithm uses a greedy set-cover strategy: it finds the smallest combination of suppliers that collectively stock every item in the order, preferring consolidation (fewer suppliers) over fragmentation.

Data is loaded at startup from embedded CSV files (products and suppliers). The service exposes a single `POST /api/route` endpoint documented via OpenAPI v3 / Swagger UI.

The solution can be opened in **Visual Studio Code** or **Visual Studio 2022** — open `Synapse.Order.Router.sln` at the repo root.

---

## Folder Structure

```
assessment/
├── Synapse.Order.Router.sln            # Open this in Visual Studio 2022
|
├── tasks/                              # Assignment instructions and source data files
│
├── Synapse.Order.Router/               # Main Azure Function project
│   ├── API/
│   │   ├── RouterController.cs         # HTTP trigger — POST /api/route
│   │   └── RouteOrderRequestExample.cs # Swagger request examples
│   ├── DataProviders/
│   │   ├── Product/
│   │   │   ├── IProductDataProvider.cs
│   │   │   ├── ProductCsvRecord.cs
│   │   │   ├── ProductDataProvider.cs
│   │   │   └── products.csv            # Embedded resource
│   │   └── Supplier/
│   │       ├── ISupplierDataProvider.cs
│   │       ├── SupplierCsvRecord.cs
│   │       ├── SupplierDataProvider.cs
│   │       └── suppliers.csv           # Embedded resource
│   ├── Models/
│   │   ├── OrderRequest.cs
│   │   ├── Product.cs
│   │   ├── RoutingResult.cs
│   │   └── Supplier.cs
│   ├── Router/
│   │   ├── RouterEngine.cs             # Pure routing logic (no I/O)
│   │   └── RouterService.cs            # DI-injected service; single try/catch boundary
│   ├── Program.cs                      # DI wiring + OpenAPI configuration
│   ├── host.json
    ├── local.settings.json             # Local dev settings (no secrets)
│   ├── Dockerfile
│   └── .dockerignore
│
└── Synapse.Order.Router.Tests/         # xUnit test project
    ├── DataProviders/
    │   ├── EnsureProductDataProvider.cs
    │   └── EnsureSupplierDataProvider.cs
    ├── Router/
    │   ├── EnsureRouter.cs             # Theory-based routing tests
    │   └── EnsureRouterIntegration.cs  # IOC integration test
    └── testdata/
        └── sample_orders.json
```

---

## Local Development

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Azure Functions Core Tools v4](https://learn.microsoft.com/azure/azure-functions/functions-run-local)

### Run the function locally

```bash
cd Synapse.Order.Router
func start
```

Swagger UI: **http://localhost:7080/api/swagger/ui**  
OpenAPI JSON: **http://localhost:7080/api/openapi/v3.json**  
Route endpoint: `POST http://localhost:7080/api/route`

> Port 7080 is set in `local.settings.json` / `launchSettings.json`. Change `Values.FUNCTIONS_WORKER_RUNTIME` to `dotnet-isolated` if needed.

### Run tests

```bash
cd Synapse.Order.Router.Tests
dotnet test
```

---

## Deployment

### Option 1 — Azure Function App

```bash
# Create resources (once)
az group create --name rg-synapse --location australiaeast
az storage account create --name synapsestore --resource-group rg-synapse --sku Standard_LRS
az functionapp create \
  --name synapse-order-router \
  --resource-group rg-synapse \
  --storage-account synapsestore \
  --consumption-plan-location australiaeast \
  --runtime dotnet-isolated \
  --runtime-version 8 \
  --functions-version 4

# Publish
cd Synapse.Order.Router
func azure functionapp publish synapse-order-router
```

### Option 2 — Docker Container

```bash
# Build image (run from Synapse.Order.Router/)
docker build -t synapse-order-router .

# Run container
docker run -d \
  --name synapse-order-router \
  -p 7071:80 \
  -e AzureWebJobsStorage="" \
  -e FUNCTIONS_WORKER_RUNTIME="dotnet-isolated" \
  synapse-order-router
```

Swagger UI: **http://localhost:7071/api/swagger/ui**

```bash
# Stop / remove
docker stop synapse-order-router
docker rm synapse-order-router
```

To push to a registry and deploy to Azure Container Apps or AKS, tag and push the image before deploying:

```bash
docker tag synapse-order-router <registry>.azurecr.io/synapse-order-router:latest
docker push <registry>.azurecr.io/synapse-order-router:latest
```
