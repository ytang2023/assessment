namespace Synapse.Order.Router.Models;

public sealed class Supplier
{
    public string SupplierId { get; init; } = "";
    public string SupplierName { get; init; } = "";
    public IReadOnlyList<(int Start, int End)> ServiceZipRanges { get; init; } = [];
    public IReadOnlySet<string> ProductCategories { get; init; } = new HashSet<string>();
    public double? CustomerSatisfactionScore { get; init; }
    public bool CanMailOrder { get; init; }

    public bool ServesZip(string zip) =>
        int.TryParse(zip, out var zipInt) &&
        ServiceZipRanges.Any(r => r.Start <= zipInt && zipInt <= r.End);
}
