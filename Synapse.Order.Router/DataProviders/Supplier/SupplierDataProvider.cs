using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Models = Synapse.Order.Router.Models;

namespace Synapse.Order.Router.DataProviders.Supplier;

public sealed class SupplierDataProvider : ISupplierDataProvider
{
    private static readonly Lazy<IReadOnlyList<Models.Supplier>> _cache = new(Load);

    public IReadOnlyList<Models.Supplier> GetSuppliers() => _cache.Value;

    private static IReadOnlyList<Models.Supplier> Load()
    {
        using var stream = typeof(SupplierDataProvider).Assembly
            .GetManifestResourceStream("Synapse.Order.Router.DataProviders.Supplier.suppliers.csv")!;
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

        return csv.GetRecords<SupplierCsvRecord>()
            .Select(Parse)
            .ToList()
            .AsReadOnly();
    }

    private static Models.Supplier Parse(SupplierCsvRecord r) => new()
    {
        SupplierId = r.SupplierId.Trim(),
        SupplierName = r.SupplierName.Trim(),
        ServiceZipRanges = ParseZipRanges(r.ServiceZips),
        ProductCategories = ParseCategories(r.ProductCategories),
        CustomerSatisfactionScore = ParseScore(r.CustomerSatisfactionScore),
        CanMailOrder = r.CanMailOrder.Trim().Equals("y", StringComparison.OrdinalIgnoreCase)
    };

    private static IReadOnlyList<(int Start, int End)> ParseZipRanges(string raw)
    {
        var ranges = new List<(int, int)>();

        foreach (var token in raw.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmed = token.Trim();
            if (TryParseRange(trimmed, out var range))
                ranges.Add(range);
            else if (int.TryParse(trimmed, out var single))
                ranges.Add((single, single));
        }

        return ranges.AsReadOnly();
    }

    private static bool TryParseRange(string token, out (int Start, int End) range)
    {
        range = default;
        var idx = token.IndexOf('-');

        if (idx <= 0) return false;

        var left = token[..idx].Trim();
        var right = token[(idx + 1)..].Trim();

        if (!int.TryParse(left, out var start) || !int.TryParse(right, out var end))
            return false;

        range = (start, end);
        return true;
    }

    private static IReadOnlySet<string> ParseCategories(string raw) =>
        raw.Split(',', StringSplitOptions.RemoveEmptyEntries)
           .Select(c => c.Trim().ToLowerInvariant())
           .ToHashSet();

    private static double? ParseScore(string raw) =>
        double.TryParse(raw.Trim(), CultureInfo.InvariantCulture, out var score) ? score : null;
}
