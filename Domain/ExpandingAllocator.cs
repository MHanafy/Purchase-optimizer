using Gluh.TechnicalTest.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gluh.TechnicalTest.Domain
{
    /// <summary>
    /// Allocates to existing POs if their vendors offer the cheapest price without incurring shipping cost
    /// </summary>
    public class ExpandingAllocator : AllocatorBase
    {
        public ExpandingAllocator(int priority = 10) : base(priority)
        {
        }

        private HashSet<Supplier> _existingSuppliers;
        protected override void OnAllocating(IRequirementBatch batch)
        {
            _existingSuppliers = batch.PurchaseOrders.Select(x => x.Supplier).Distinct().ToHashSet();
            base.OnAllocating(batch);
        }

        protected override bool ShouldAllocate(Stock stock, int quantity)
        {
            return _existingSuppliers.Contains(stock.Supplier);
        }

    }
}
