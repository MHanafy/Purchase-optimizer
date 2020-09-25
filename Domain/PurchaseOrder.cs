using Gluh.TechnicalTest.Database;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Gluh.TechnicalTest.Domain
{

    public interface IUnfulfilledOrder
    {
        IImmutableList<IPurchaseOrderLine> Lines { get; }
    }

    public interface IPurchaseOrder : IUnfulfilledOrder
    {
        Supplier Supplier { get; }
        decimal SubTotal { get; }
        decimal Shipping { get; }
        decimal Total { get; }
        /// <summary>
        /// Calculates the effective cost of adding a new line, without adding the line.
        /// Helpful for when adding a line changes PO shipping cost, discounts, etc..
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        decimal CalculateEffectiveCost(IPurchaseOrderLine line);
        void Add(IPurchaseOrderLine line);
    }

    public class PurchaseOrder : IPurchaseOrder
    {
        public Supplier Supplier { get; private set; }

        private readonly List<IPurchaseOrderLine> _lines;
        public IImmutableList<IPurchaseOrderLine> Lines => _lines.ToImmutableList();

        public decimal SubTotal { get; private set; }

        public decimal Shipping { get; private set; }

        public decimal Total => SubTotal + Shipping;

        /// <summary>
        /// Indicates there's at least a single line for a shipping eligible product.
        /// </summary>
        private bool _shippingEligible;
        public void Add(IPurchaseOrderLine line)
        {
            _lines.Add(line);
            //This flag changes to true only, as we don't support removing lines.
            if (line.Product.ShippingEligible) _shippingEligible = true;
            SubTotal += line.Total;
            if (_shippingEligible)
            {
                Shipping = CalculateShipping(SubTotal);
            }
        }

        private decimal CalculateShipping(decimal subTotal)
        {
            if (subTotal >= Supplier.ShippingCostMinOrderValue || subTotal <= Supplier.ShippingCostMaxOrderValue) return 0;
            return Supplier.ShippingCost;
        }

        public decimal CalculateEffectiveCost(IPurchaseOrderLine line)
        {
            if (!_shippingEligible && !line.Product.ShippingEligible) return line.Total;
            var newSubtotal = SubTotal + line.Total;
            var shipping = CalculateShipping(newSubtotal);
            var newTotal = newSubtotal + shipping;
            return newTotal - Total;
        }
    }
}
