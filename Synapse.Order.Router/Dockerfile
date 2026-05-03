FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Synapse.Order.Router.csproj", "."]
RUN dotnet restore "Synapse.Order.Router.csproj"
COPY . .
RUN dotnet publish "Synapse.Order.Router.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true
COPY --from=build /app/publish .
