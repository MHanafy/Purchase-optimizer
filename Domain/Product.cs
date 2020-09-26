using Gluh.TechnicalTest.Database;

namespace Gluh.TechnicalTest.Domain
{
    public interface IProduct
    {
        int Id { get; }
        string Name { get; }
        ProductType Type { get; }
        /// <summary>
        /// Indicates that the product is eligible for a shipping cost, typically indicates a physical product.
        /// </summary>
        bool ShippingEligible { get; }
    }

    public class Product : IProduct
    {
        public Product(int id, string name, ProductType type)
        {
            Id = id;
            Name = name;
            Type = type;
        }
        public int Id { get; private set; }
        public string Name { get; private set; }
        public ProductType Type { get; private set; }
        //This is virtual incase a derived product needs to have a different rule.
        public virtual bool ShippingEligible => Type == ProductType.Physical;

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
