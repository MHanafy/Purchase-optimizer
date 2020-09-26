using System;

namespace Gluh.TechnicalTest.Domain
{
    public interface IAllocator
    {
        void Allocate(IRequirementBatch batch);
        /// <summary>
        /// Defines order of execution, lower priority executes first.
        /// </summary>
        int Priority { get; }
    }

    public abstract class AllocatorBase : IAllocator
    {
        public AllocatorBase(int priority = 0)
        {
            Priority = priority;
        }
        public int Priority { get; }
        protected virtual bool ShouldAllocate(Stock stock, int quantity) => true;
        protected virtual void OnAllocating(IRequirementBatch batch) { }
        public virtual void Allocate(IRequirementBatch batch)
        {
            OnAllocating(batch);
            foreach (var req in batch.Unfulfilled)
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

    }
}
