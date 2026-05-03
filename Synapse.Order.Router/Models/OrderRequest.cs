using System.Text.Json.Serialization;

namespace Synapse.Order.Router.Models;

public sealed class OrderRequest
{
    [JsonPropertyName("order_id")]
    public string OrderId { get; init; } = "";

    [JsonPropertyName("customer_zip")]
    public string CustomerZip { get; init; } = "";

    [JsonPropertyName("mail_order")]
    public bool MailOrder { get; init; }

    [JsonPropertyName("items")]
    public IReadOnlyList<OrderItem> Items { get; init; } = [];
}

public sealed class OrderItem
{
    [JsonPropertyName("product_code")]
    public string ProductCode { get; init; } = "";

    [JsonPropertyName("quantity")]
    public int Quantity { get; init; }
}
