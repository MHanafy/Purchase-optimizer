using Gluh.TechnicalTest.Database;

namespace Gluh.TechnicalTest.Domain
{

    public interface IPurchaseOrderLine
    {
        IProduct Product { get; }
        int Quantity { get; }
        decimal Total { get; }
    }

    public class PurchaseOrderLine : IPurchaseOrderLine
    {
        public PurchaseOrderLine(Supplier supplier, IProduct product, int quantity)
        {
            Supplier = supplier;
            Product = product;
            Quantity = quantity;
        }
        Supplier Supplier { get; }

        public IProduct Product { get; private set; }

        public int Quantity {get; private set; }

        public decimal Total => Product.Cost * Quantity;
    }
}
