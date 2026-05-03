using System.Text.RegularExpressions;
using Synapse.Order.Router.Models;

namespace Synapse.Order.Router.Router;

public sealed class RouterEngine
{
    private const double DefaultScore = 5.0;

    public RoutingResult Route(
        OrderRequest order,
        IReadOnlyList<Supplier> suppliers,
        IReadOnlyDictionary<string, Product> productsByCode)
    {
        var errors = Validate(order, productsByCode);
        if (errors.Count > 0)
            return new RoutingResult { Feasible = false, Errors = errors };

        var itemsWithProduct = order.Items
            .Select(item => (Item: item, Product: productsByCode[item.ProductCode]))
            .ToList();

        var eligibleByItem = itemsWithProduct.ToDictionary(
            x => x.Item.ProductCode,
            x => GetEligibleSuppliers(x.Product.Category, order, suppliers));

        var infeasibleErrors = eligibleByItem
            .Where(kv => kv.Value.Count == 0)
            .Select(kv => $"No eligible supplier found for product '{kv.Key}'.")
            .ToList();

        if (infeasibleErrors.Count > 0)
            return new RoutingResult { Feasible = false, Errors = infeasibleErrors };

        return new RoutingResult { Feasible = true, Routing = Assign(itemsWithProduct, eligibleByItem) };
    }

    private static List<string> Validate(
        OrderRequest order,
        IReadOnlyDictionary<string, Product> productsByCode)
    {
        var errors = new List<string>();

        if (order.Items.Count == 0)
            errors.Add("Order must include at least one line item.");

        if (string.IsNullOrWhiteSpace(order.CustomerZip) || !Regex.IsMatch(order.CustomerZip, @"^\d{5}$"))
            errors.Add("Order must include a valid customer_zip.");

        foreach (var item in order.Items.Where(i => !productsByCode.ContainsKey(i.ProductCode)))
            errors.Add($"Unknown product code '{item.ProductCode}'.");

        return errors;
    }

    private static List<EligibleSupplier> GetEligibleSuppliers(
        string category,
        OrderRequest order,
        IReadOnlyList<Supplier> suppliers) =>
        suppliers
            .Where(s => s.ProductCategories.Contains(category))
            .Select(s => new
            {
                Supplier = s,
                IsLocal = s.ServesZip(order.CustomerZip),
                Eligible = order.MailOrder ? s.CanMailOrder : s.ServesZip(order.CustomerZip)
            })
            .Where(x => x.Eligible)
            .Select(x => new EligibleSupplier(x.Supplier, x.IsLocal, x.Supplier.CustomerSatisfactionScore ?? DefaultScore))
            .ToList();

    private static IReadOnlyList<SupplierAssignment> Assign(
        List<(OrderItem Item, Product Product)> itemsWithProduct,
        Dictionary<string, List<EligibleSupplier>> eligibleByItem)
    {
        var commonIds = eligibleByItem.Values
            .Select(list => list.Select(e => e.Supplier.SupplierId).ToHashSet())
            .Aggregate((a, b) => a.Intersect(b).ToHashSet());

        if (commonIds.Count > 0)
        {
            var best = eligibleByItem.Values.First()
                .Where(e => commonIds.Contains(e.Supplier.SupplierId))
                .OrderByDescending(e => e.Score)
                .ThenByDescending(e => e.IsLocal ? 1 : 0)
                .First();

            return [BuildAssignment(best, itemsWithProduct)];
        }

        return GreedyAssign(itemsWithProduct, eligibleByItem);
    }

    private static IReadOnlyList<SupplierAssignment> GreedyAssign(
        List<(OrderItem Item, Product Product)> itemsWithProduct,
        Dictionary<string, List<EligibleSupplier>> eligibleByItem)
    {
        var unassigned = itemsWithProduct.Select(x => x.Item.ProductCode).ToHashSet();
        var assignments = new List<SupplierAssignment>();

        while (unassigned.Count > 0)
        {
            var best = PickBestSupplier(unassigned, eligibleByItem);

            var covered = itemsWithProduct
                .Where(x => unassigned.Contains(x.Item.ProductCode) &&
                             eligibleByItem[x.Item.ProductCode].Any(e => e.Supplier.SupplierId == best.Supplier.SupplierId))
                .ToList();

            assignments.Add(BuildAssignment(best, covered));

            foreach (var x in covered)
                unassigned.Remove(x.Item.ProductCode);
        }

        return assignments.AsReadOnly();
    }

    private static EligibleSupplier PickBestSupplier(
        HashSet<string> unassigned,
        Dictionary<string, List<EligibleSupplier>> eligibleByItem)
    {
        var coverage = new Dictionary<string, (EligibleSupplier Eligible, int Count)>();

        foreach (var code in unassigned)
        foreach (var eligible in eligibleByItem[code])
        {
            var id = eligible.Supplier.SupplierId;
            coverage[id] = coverage.TryGetValue(id, out var existing)
                ? (existing.Eligible, existing.Count + 1)
                : (eligible, 1);
        }

        return coverage.Values
            .OrderByDescending(x => x.Count)
            .ThenByDescending(x => x.Eligible.Score)
            .ThenByDescending(x => x.Eligible.IsLocal ? 1 : 0)
            .First()
            .Eligible;
    }

    private static SupplierAssignment BuildAssignment(
        EligibleSupplier eligible,
        IEnumerable<(OrderItem Item, Product Product)> items) =>
        new()
        {
            SupplierId = eligible.Supplier.SupplierId,
            SupplierName = eligible.Supplier.SupplierName,
            Items = items.Select(x => new RoutedItem
            {
                ProductCode = x.Item.ProductCode,
                Quantity = x.Item.Quantity,
                Category = x.Product.Category,
                FulfillmentMode = eligible.IsLocal ? "local" : "mail_order"
            }).ToList().AsReadOnly()
        };

    private sealed record EligibleSupplier(Supplier Supplier, bool IsLocal, double Score);
}
