using Gluh.TechnicalTest.Database;
using System.Collections.Generic;

namespace Gluh.TechnicalTest.Domain
{
    public interface IRequirementBatch
    {
        IEnumerable<IRequirement> Unallocated { get; }
        int AddPurchaseOrderLine(IPurchaseOrderLine line);
        int AddPurchaseOrderLine(Supplier supplier, IProduct product, decimal price, int quantity);
        IUnfulfilledOrder UnfulfilledOrder { get; }
        IEnumerable<IPurchaseOrder> PurchaseOrders { get; }
        void ProcessNoStock();
        HashSet<Supplier> AllocatedSuppliers { get; }
        /// <summary>
        /// Calculates the total effective cost, including shipping if given lines are added.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        decimal CalculateEffectiveCost(IEnumerable<IPurchaseOrderLine> lines);
        /// <summary>
        /// Returns available stock cheapest first.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        IEnumerable<Stock> GetAvailableStock(IProduct product, Supplier supplier = null);
        int UnallocatedCount { get; }
        int UnfulfilledCount { get; }
        int AllocatedCount { get; }
    }
}
