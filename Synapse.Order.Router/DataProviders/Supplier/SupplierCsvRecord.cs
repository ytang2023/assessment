using CsvHelper.Configuration.Attributes;

namespace Synapse.Order.Router.DataProviders.Supplier;

internal sealed class SupplierCsvRecord
{
    [Name("supplier_id")]
    public string SupplierId { get; set; } = "";

    [Name("suplier_name")] // note: typo in CSV header
    public string SupplierName { get; set; } = "";

    [Name("service_zips")]
    public string ServiceZips { get; set; } = "";

    [Name("product_categories")]
    public string ProductCategories { get; set; } = "";

    [Name("customer_satisfaction_score")]
    public string CustomerSatisfactionScore { get; set; } = "";

    [Name("can_mail_order?")]
    public string CanMailOrder { get; set; } = "";
}
