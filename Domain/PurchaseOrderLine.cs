﻿using Gluh.TechnicalTest.Database;

namespace Gluh.TechnicalTest.Domain
{

    public interface IPurchaseOrderLine : IOrderLineBase
    {
        decimal Total { get; }
        Supplier Supplier { get; }
    }

    public class PurchaseOrderLine : OrderLineBase, IPurchaseOrderLine
    {
        public PurchaseOrderLine(Supplier supplier, IProduct product, int quantity) : base(product, quantity)
        {
            Supplier = supplier;
        }
        public Supplier Supplier { get;}

        public decimal Total => Product.Cost * Quantity;

        public override string ToString()
        {
            return $"{Supplier.Name} - {Quantity} - {Total:c}";
        }
    }
}
