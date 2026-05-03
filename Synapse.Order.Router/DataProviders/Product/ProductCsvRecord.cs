using CsvHelper.Configuration.Attributes;

namespace Synapse.Order.Router.DataProviders.Product;

internal sealed class ProductCsvRecord
{
    [Name("product_code")]
    public string ProductCode { get; set; } = "";

    [Name("product_name")]
    public string ProductName { get; set; } = "";

    [Name("category")]
    public string Category { get; set; } = "";
}
