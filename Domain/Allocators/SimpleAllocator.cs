using System;

namespace Gluh.TechnicalTest.Domain.Allocators
{
    /// <summary>
    /// Provides a basis for all allocators that act upon individual requirements
    /// </summary>
    public abstract class SimpleAllocator : AllocatorBase
    {
        public SimpleAllocator(int priority = 0) : base(priority)
        {
        }
        protected override void AllocateAll(IRequirementBatch batch)
        {
            foreach (var req in batch.Unallocated)
            {
                var stocks = batch.Cache.GetAvailableStock(req.Product);
                var allocated = 0;
                foreach (var stock in stocks)
                {
                    var quantity = Math.Min(req.Quantity - allocated, stock.Quantity);
                    if (ShouldAllocate(stock, quantity))
                    {
                        allocated += quantity;
                        batch.AddPurchaseOrderLine(stock.Supplier, stock.Product, stock.Cost, quantity);
                        if (req.Quantity == allocated) break;
                    }
                    else break;
                }
            }
        }
        protected virtual bool ShouldAllocate(Stock stock, int quantity) => true;
    }
}
