using Gluh.TechnicalTest.Database;

namespace Gluh.TechnicalTest.Domain
{
    public interface IProduct
    {
        int Id { get; }
        string Name { get; }
        ProductType Type { get; }
        
        decimal Cost { get; }
        /// <summary>
        /// Indicates that the product is eligible for a shipping cost, typically indicates a physical product.
        /// </summary>
        bool ShippingEligible { get; }
    }

    public class Product : IProduct
    {
        public Product(int id, string name, decimal cost, ProductType type)
        {
            Id = id;
            Name = name;
            Cost = cost;
            Type = type;
        }
        public int Id { get; private set; }
        public string Name { get; private set; }
        public decimal Cost { get; private set; }
        public ProductType Type { get; private set; }
        //This is virtual incase a derived product needs to have a different rule.
        public virtual bool ShippingEligible => Type == ProductType.Physical;

        public override string ToString()
        {
            return $"{Name} - {Cost:c}";
        }
    }
}
