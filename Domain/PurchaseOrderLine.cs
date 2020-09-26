using Gluh.TechnicalTest.Database;

namespace Gluh.TechnicalTest.Domain
{

    public interface IPurchaseOrderLine : IOrderLineBase
    {
        decimal Price { get; }
        decimal Total { get; }
        Supplier Supplier { get; }
    }

    public class PurchaseOrderLine : OrderLineBase, IPurchaseOrderLine
    {
        public PurchaseOrderLine(Supplier supplier, IProduct product, decimal price, int quantity) : base(product, quantity)
        {
            Supplier = supplier;
            Price = price;
        }
        public decimal Price { get; }
        public Supplier Supplier { get;}

        public decimal Total => Price * Quantity;

        public override string ToString()
        {
            return $"{Supplier.Name} - {Quantity} - {Total:c}";
        }
    }
}
