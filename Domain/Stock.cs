using Gluh.TechnicalTest.Database;

namespace Gluh.TechnicalTest.Domain
{

    public interface IStock
    {
        IProduct Product { get; }
        int Quantity { get; }
        decimal Cost { get; }
        Supplier Supplier { get; }
    }

    public class Stock : IStock
    {
        public Stock(IProduct product, Supplier supplier, int quantity, decimal cost)
        {
            Product = product;
            Supplier = supplier;
            Quantity = quantity;
            Cost = cost;
        }
        public IProduct Product { get; }

        public int Quantity { get; set; }

        public decimal Cost { get; }

        public Supplier Supplier { get;  }

        public override string ToString()
        {
            return $"{Supplier} - {Quantity} at {Cost:c}";
        }
    }

}
