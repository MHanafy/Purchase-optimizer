using System.Collections.Generic;

namespace Gluh.TechnicalTest.Domain
{

    public interface IRequirement
    {
        IProduct Product { get; }
        int Quantity { get; set; }
        List<IPurchaseOrder> PurchaseOrders { get; }
    }
    public class Requirement : IRequirement
    {
        public Requirement(IProduct product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
        public IProduct Product { get; }

        public int Quantity { get; set; }

        public List<IPurchaseOrder> PurchaseOrders { get; }

        public override string ToString()
        {
            return $"{Quantity} x {Product}";
        }
    }
}
