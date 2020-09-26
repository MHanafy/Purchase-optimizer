using Gluh.TechnicalTest.Database;
using Gluh.TechnicalTest.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gluh.TechnicalTest.Domain
{
    public interface IRequirementBatch
    {
        IEnumerable<IRequirement> Unfulfilled { get; }
        IStockCache Cache { get; }
        int AddPurchaseOrderLine(Supplier supplier, IProduct product, decimal price, int quantity);
        IUnfulfilledOrder UnfulfilledOrder { get; }
        IEnumerable<IPurchaseOrder> PurchaseOrders { get; }
        void ProcessNoStock();
    }

    public class RequirementBatch : IRequirementBatch
    {
        private readonly Dictionary<IProduct, int> _requirements;
        private readonly IStockCache _cache;
        private readonly IUnfulfilledOrder _unfulfilledOrder;
        private readonly Dictionary<Supplier, IPurchaseOrder> _purchaseOrders;
        public RequirementBatch(List<PurchaseRequirement> requirements)
        {
            _requirements = requirements.ToDictionary(x=>x.Product.ToProduct(), y => y.Quantity);

            _cache = new StockCache(requirements);
            _unfulfilledOrder = new UnfulfilledOrder();
            _purchaseOrders = new Dictionary<Supplier, IPurchaseOrder>();
        }

        public IEnumerable<IRequirement> Unfulfilled => _requirements.Where(x => x.Value > 0)
            .Select(x=> new Requirement(x.Key, x.Value))
            .ToList();

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

        /// <summary>
        /// Removes requirements that can't be fulfilled because of no stock and adds entries to Unfulfilled.
        /// </summary>
        public void ProcessNoStock()
        {
            foreach (var req in Unfulfilled)
            {
                if(_cache.GetAvailableStock(req.Product).Sum(x=>x.Quantity) == 0)
                {
                    _unfulfilledOrder.Add(new OrderLineBase(req.Product, req.Quantity));
                    _requirements[req.Product] = 0;
                }
            }
        }
    }
}
