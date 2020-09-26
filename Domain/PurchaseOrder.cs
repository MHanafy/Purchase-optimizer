using Gluh.TechnicalTest.Database;
using System.Collections.Generic;
using System.Linq;

namespace Gluh.TechnicalTest.Domain
{
    public interface IPurchaseOrder : IOrderBase<IPurchaseOrderLine>
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

    public class PurchaseOrder : OrderBase<IPurchaseOrderLine>, IPurchaseOrder
    {
        public PurchaseOrder(Supplier supplier) : base()
        {
            Supplier = supplier;
        }
        public PurchaseOrder(Supplier supplier, IEnumerable<IPurchaseOrderLine> lines) : base(lines)
        {
            Supplier = supplier;
            _shippingEligible = _lines.Any(x => x.Product.ShippingEligible);
            SubTotal = _lines.Sum(x => x.Total);
        }

        public Supplier Supplier { get; }

        public decimal SubTotal { get; private set; }

        public decimal Shipping { get; private set; }

        public decimal Total => SubTotal + Shipping;

        /// <summary>
        /// Indicates there's at least a single line for a shipping eligible product.
        /// </summary>
        private bool _shippingEligible;


        protected override void OnLineAdded(IPurchaseOrderLine line)
        {
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
