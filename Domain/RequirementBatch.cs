using Gluh.TechnicalTest.Database;
using Gluh.TechnicalTest.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gluh.TechnicalTest.Domain
{
    public interface IRequirementBatch : IEnumerable<IRequirement>
    {
        IEnumerable<IRequirement> Unfulfilled { get; }
        IStockCache Cache { get; }
        int AddPurchaseOrderLine(Supplier supplier, IProduct product, decimal price, int quantity);
        IUnfulfilledOrder UnfulfilledOrder { get; }
        IEnumerable<IPurchaseOrder> PurchaseOrders { get; }
    }

    public class RequirementBatch : IRequirementBatch
    {
        private readonly Dictionary<IProduct, int> _requirements;
        private readonly IStockCache _cache;
        private readonly IUnfulfilledOrder _unfulfilledOrder;
        private readonly Dictionary<Supplier, IPurchaseOrder> _purchaseOrders;
        public RequirementBatch(List<PurchaseRequirement> requirements)
        {
            //_requirements = new Dictionary<IProduct, IRequirement>();
            //foreach (var req in requirements)
            //{
            //    var product = req.Product.ToProduct();
            //    var requirment = new Requirement(product, req.Quantity);
            //    _requirements.Add(product, requirment)
            //}
            _requirements = requirements.ToDictionary(x=>x.Product.ToProduct(), y => y.Quantity);

            _cache = new StockCache(requirements);
            _unfulfilledOrder = new UnfulfilledOrder();
            _purchaseOrders = new Dictionary<Supplier, IPurchaseOrder>();
        }

        public IEnumerable<IRequirement> Unfulfilled => _requirements.Where(x => x.Value > 0).Select(x=> new Requirement(x.Key, x.Value));

        public IStockCache Cache => _cache;

        public IUnfulfilledOrder UnfulfilledOrder => _unfulfilledOrder;

        public IEnumerable<IPurchaseOrder> PurchaseOrders => _purchaseOrders.Values;

        public int AddPurchaseOrderLine(Supplier supplier, IProduct product, decimal price, int quantity)
        {
            var stock = _cache.GetAvailableStock(product, supplier).FirstOrDefault();
            if ((stock?.Quantity??0) <quantity) throw new InvalidOperationException("Not enough stock to allocate!");
            var line = new PurchaseOrderLine(supplier, product, price, quantity);
            if (!_purchaseOrders.ContainsKey(supplier)) _purchaseOrders.Add(supplier, new PurchaseOrder(supplier));
            _purchaseOrders[supplier].Add(line);
            //subtract allocated quantity from available stock.
            stock.Quantity -= line.Quantity;
            _requirements[product] -= quantity;
            return _requirements[product];
        }

        public IEnumerator<IRequirement> GetEnumerator()
        {
            return _requirements.Select(x => new Requirement(x.Key, x.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
