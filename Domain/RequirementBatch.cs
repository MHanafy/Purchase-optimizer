using Gluh.TechnicalTest.Database;
using Gluh.TechnicalTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gluh.TechnicalTest.Domain
{
    public interface IRequirementBatch
    {
        IEnumerable<IRequirement> Unallocated { get; }
        IStockCache Cache { get; }
        int AddPurchaseOrderLine(IPurchaseOrderLine line);
        int AddPurchaseOrderLine(Supplier supplier, IProduct product, decimal price, int quantity);
        IUnfulfilledOrder UnfulfilledOrder { get; }
        IEnumerable<IPurchaseOrder> PurchaseOrders { get; }
        void ProcessNoStock();
        HashSet<Supplier> AllocatedSuppliers { get; }
        /// <summary>
        /// Calculates the total effective cost, including shipping if given lines are added.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        decimal CalculateEffectiveCost(IEnumerable<IPurchaseOrderLine> lines);
    }

    public class RequirementBatch : IRequirementBatch
    {
        private readonly Dictionary<IProduct, int> _requirements;
        private readonly Dictionary<Supplier, IPurchaseOrder> _purchaseOrders;
        public RequirementBatch(List<PurchaseRequirement> requirements)
        {
            UnfulfilledOrder = new UnfulfilledOrder();
            _purchaseOrders = new Dictionary<Supplier, IPurchaseOrder>();
            _requirements = requirements.ToDictionary(x=>x.Product.ToProduct(), y => y.Quantity);

            Cache = new StockCache(requirements);
        }

        public HashSet<Supplier> AllocatedSuppliers => _purchaseOrders.Keys.ToHashSet();
        public IEnumerable<IRequirement> Unallocated => _requirements.Where(x => x.Value > 0)
            .Select(x=> new Requirement(x.Key, x.Value))
            .ToList();

        public IStockCache Cache { get; }
        public IUnfulfilledOrder UnfulfilledOrder { get; }

        public IEnumerable<IPurchaseOrder> PurchaseOrders => _purchaseOrders.Values;

        public int AllocatedLineCount => _purchaseOrders.Sum(x => x.Value.Lines.Count);
        public int AllocatedCount => _purchaseOrders.Sum(x => x.Value.Lines.Sum(l => l.Quantity));

        public int UnallocatedCount => Unallocated.Sum(x => x.Quantity);
        public int UnfulfilledCount => UnfulfilledOrder.Lines.Sum(x => x.Quantity);

        public int AddPurchaseOrderLine(Supplier supplier, IProduct product, decimal price, int quantity)
        {
            var line = new PurchaseOrderLine(supplier, product, price, quantity);
            return AddPurchaseOrderLine(line);
        }

        public int AddPurchaseOrderLine(IPurchaseOrderLine line)
        {
            var stock = Cache.GetAvailableStock(line.Product, line.Supplier).FirstOrDefault();
            if ((stock?.Quantity??0) < line.Quantity) throw new InvalidOperationException("Not enough stock to allocate!");
            if (!_purchaseOrders.ContainsKey(line.Supplier)) _purchaseOrders.Add(line.Supplier, new PurchaseOrder(line.Supplier));
            _purchaseOrders[line.Supplier].Add(line);
            //subtract allocated quantity from available stock.
            stock.Quantity -= line.Quantity;
            _requirements[line.Product] -= line.Quantity;
            AllocatedSuppliers.Add(line.Supplier);
            return _requirements[line.Product];
        }

        /// <summary>
        /// Removes requirements that can't be fulfilled because of no stock and adds entries to Unfulfilled.
        /// </summary>
        public void ProcessNoStock()
        {
            foreach (var req in Unallocated)
            {
                if(Cache.GetAvailableStock(req.Product).Sum(x=>x.Quantity) == 0)
                {
                    UnfulfilledOrder.Add(new OrderLineBase(req.Product, req.Quantity));
                    _requirements[req.Product] = 0;
                }
            }
        }

        public decimal CalculateEffectiveCost(IEnumerable<IPurchaseOrderLine> lines)
        {
            decimal total = 0;
            var unallocated = new List<IPurchaseOrderLine>();
            foreach (var line in lines)
            {
                if (!_purchaseOrders.Keys.Contains(line.Supplier))
                {
                    unallocated.Add(line);
                    continue;
                }
                total += _purchaseOrders[line.Supplier].CalculateEffectiveCost(line);
            }
            var newOrders = unallocated.GroupBy(x => x.Supplier).Select(x=> new PurchaseOrder(x.Key, x));
            total += newOrders.Sum(x => x.Total);
            return total;
        }
    }
}
