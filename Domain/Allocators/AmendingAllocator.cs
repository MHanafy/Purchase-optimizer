using Gluh.TechnicalTest.Database;
using Gluh.TechnicalTest.Domain.Allocators;
using System.Collections.Generic;

namespace Gluh.TechnicalTest.Domain
{
    /// <summary>
    /// Allocates to existing POs if their vendors offer the cheapest price without incurring shipping cost
    /// </summary>
    public class AmendingAllocator : SimpleAllocator
    {
        public AmendingAllocator(int priority = 10) : base(priority)
        {
        }

        private HashSet<Supplier> _existingSuppliers;
        protected override void OnAllocating(IRequirementBatch batch)
        {
            _existingSuppliers = batch.AllocatedSuppliers;
            base.OnAllocating(batch);
        }

        protected override bool ShouldAllocate(Stock stock, int quantity)
        {
            return _existingSuppliers.Contains(stock.Supplier);
        }

    }
}
