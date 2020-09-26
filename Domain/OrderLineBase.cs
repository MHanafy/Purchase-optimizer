namespace Gluh.TechnicalTest.Domain
{
    public interface IOrderLineBase
    {
        IProduct Product { get; }
        int Quantity { get; }
    }

    public class OrderLineBase : IOrderLineBase
    {
        public OrderLineBase(IProduct product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public IProduct Product { get; }
        public int Quantity { get; }
    }

}
