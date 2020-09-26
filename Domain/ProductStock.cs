using Gluh.TechnicalTest.Database;

namespace Gluh.TechnicalTest.Domain
{

    public interface IProductStock
    {
        Product Product { get; }
        int Quantity { get; }
        decimal Cost { get; }
        Supplier Supplier { get; }
    }

    public class ProductStock : IProductStock
    {
        public ProductStock(Product product, Supplier supplier, int quantity, decimal cost)
        {
            Product = product;
            Supplier = supplier;
            Quantity = quantity;
            Cost = cost;
        }
        public Product Product { get; }

        public int Quantity { get; set; }

        public decimal Cost { get; }

        public Supplier Supplier { get;  }
    }

}
