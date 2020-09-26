using Gluh.TechnicalTest.Database;
using System;
using System.Diagnostics.CodeAnalysis;

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

    public class Product : IProduct, IEquatable<Product>
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

        public bool Equals([AllowNull] Product other)
        {
            if (other == null) return false;
            //relying on the Id for now
            return Id == other.Id;
        }
        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            Product product = obj as Product;
            if (product == null)
                return false;
            else
                return Equals(product);
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
