using Gluh.TechnicalTest.Database;
using Gluh.TechnicalTest.Domain;
using NSubstitute;
using NUnit.Framework;
using System.Collections.Generic;

namespace Gluh.TechnicalTest.Tests
{
    public class NoShippingAllocatorTests
    {
        [Test]
        public void Allocate_FreeShipping_AllocatesToCheapest()
        {
            //Arrange
            var allocator = new NoShippingAllocator();
            var batch = Substitute.For<IRequirementBatch>();
            var product = Substitute.For<IProduct>();
            var requirements = new List<IRequirement> {new Requirement(product, 5) };
            var cheapestSupplier = new Supplier() { ID = 1, ShippingCost = 0 };
            var supplier1 = new Supplier() { ID = 2, ShippingCost = 0 };
            var stock = new List<Stock> {
                new Stock(product, cheapestSupplier, 10, 9.9m),
                new Stock(product, supplier1, 10, 10)
            };
            batch.Unallocated.Returns(requirements);
            batch.GetAvailableStock(product).Returns(stock);

            //Act
            allocator.Allocate(batch);

            //Assert
            batch.Received().AddPurchaseOrderLine(cheapestSupplier, product, 9.9m, 5);
        }

        [Test]
        public void Allocate_FreeShippingInadequateQuantity_AllocatesToCheapestMultiple()
        {
            //Arrange
            var allocator = new NoShippingAllocator();
            var batch = Substitute.For<IRequirementBatch>();
            var product = Substitute.For<IProduct>();
            var requirements = new List<IRequirement> { new Requirement(product, 5) };
            var cheapestSupplier = new Supplier() { ID = 1, ShippingCost = 0 };
            var supplier1 = new Supplier() { ID = 2, ShippingCost = 0 };
            var stock = new List<Stock> {
                new Stock(product, cheapestSupplier, 2, 9.9m),
                new Stock(product, supplier1, 10, 10)
            };
            batch.Unallocated.Returns(requirements);
            batch.GetAvailableStock(product).Returns(stock);

            //Act
            allocator.Allocate(batch);

            //Assert
            batch.Received().AddPurchaseOrderLine(cheapestSupplier, product, 9.9m, 2);
            batch.Received().AddPurchaseOrderLine(supplier1, product, 10m, 3);
        }

        [Test]
        [TestCase(10, 5)]
        [TestCase(0.1, 5)]
        public void Allocate_NoFreeshipping_NoAllocatations(decimal shippingCost, int quantity)
        {
            //Arrange
            var allocator = new NoShippingAllocator();
            var batch = Substitute.For<IRequirementBatch>();
            var product = Substitute.For<IProduct>();
            var requirements = new List<IRequirement> { new Requirement(product, quantity) };
            var cheapestSupplier = new Supplier() { ID = 1, ShippingCost = shippingCost, ShippingCostMaxOrderValue = 99999 };
            var supplier1 = new Supplier() { ID = 2, ShippingCost = shippingCost, ShippingCostMaxOrderValue = 99999 };
            var stock = new List<Stock> {
                new Stock(product, cheapestSupplier, 2, 9.9m),
                new Stock(product, supplier1, 10, 10)
            };
            batch.Unallocated.Returns(requirements);
            batch.GetAvailableStock(product).Returns(stock);

            //Act
            allocator.Allocate(batch);

            //Assert
            batch.DidNotReceiveWithAnyArgs().AddPurchaseOrderLine(Arg.Any<Supplier>(), Arg.Any<IProduct>(), Arg.Any<decimal>(), Arg.Any<int>());
        }
    }
}