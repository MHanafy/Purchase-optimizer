using Gluh.TechnicalTest.Domain.Allocators;

namespace Gluh.TechnicalTest.Domain
{

    /// <summary>
    /// <para>Tries to fulfull all possible requirements from the cheapest suppliers that either have no shipping cost, or has free shipping for the requirement.</para>
    /// i.e. shipping cost is zero, or total exceeds ShippingCostMaxOrderValue, hence can't guarantee full allocation
    /// </summary>
    class NoShippingAllocator : SimpleAllocator
    {
        public NoShippingAllocator(int priority = 10) : base(priority)
        {
        }

        protected override bool ShouldAllocate(Stock stock, int quantity)
        {
            var cost = quantity * stock.Cost;
            if (stock.Supplier.ShippingCost == 0 || stock.Supplier.ShippingCostMaxOrderValue < cost) return true;
            return false;
        }
    }
}
