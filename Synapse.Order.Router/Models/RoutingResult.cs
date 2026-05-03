using System.Text.Json.Serialization;

namespace Synapse.Order.Router.Models;

public sealed class RoutingResult
{
    [JsonPropertyName("feasible")]
    public bool Feasible { get; init; }

    [JsonPropertyName("routing")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<SupplierAssignment>? Routing { get; init; }

    [JsonPropertyName("errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? Errors { get; init; }
}

public sealed class SupplierAssignment
{
    [JsonPropertyName("supplier_id")]
    public string SupplierId { get; init; } = "";

    [JsonPropertyName("supplier_name")]
    public string SupplierName { get; init; } = "";

    [JsonPropertyName("items")]
    public IReadOnlyList<RoutedItem> Items { get; init; } = [];
}

public sealed class RoutedItem
{
    [JsonPropertyName("product_code")]
    public string ProductCode { get; init; } = "";

    [JsonPropertyName("quantity")]
    public int Quantity { get; init; }

    [JsonPropertyName("category")]
    public string Category { get; init; } = "";

    [JsonPropertyName("fulfillment_mode")]
    public string FulfillmentMode { get; init; } = "";
}
