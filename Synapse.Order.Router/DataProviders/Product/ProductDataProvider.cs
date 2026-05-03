using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Models = Synapse.Order.Router.Models;

namespace Synapse.Order.Router.DataProviders.Product;

public sealed class ProductDataProvider : IProductDataProvider
{
    private static readonly Lazy<IReadOnlyDictionary<string, Models.Product>> _cache = new(Load);

    public IReadOnlyDictionary<string, Models.Product> GetProducts() => _cache.Value;

    private static IReadOnlyDictionary<string, Models.Product> Load()
    {
        using var stream = typeof(ProductDataProvider).Assembly
            .GetManifestResourceStream("Synapse.Order.Router.DataProviders.Product.products.csv")!;
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

        var dict = new Dictionary<string, Models.Product>(StringComparer.OrdinalIgnoreCase);
        foreach (var r in csv.GetRecords<ProductCsvRecord>())
        {
            var product = new Models.Product
            {
                ProductCode = r.ProductCode.Trim(),
                ProductName = r.ProductName.Trim(),
                Category = r.Category.Trim().ToLowerInvariant()
            };
            dict[product.ProductCode] = product;  // last-write-wins for duplicates
        }
        return dict;
    }
}
