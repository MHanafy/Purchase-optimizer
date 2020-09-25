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
        public PurchaseOrderLine(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
        public IProduct Product { get; private set; }

        public int Quantity {get; private set; }

        public decimal Total => Product.Cost * Quantity;
    }
}
