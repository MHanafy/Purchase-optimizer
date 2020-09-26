using System;
using System.Collections.Generic;
using System.Text;
using Gluh.TechnicalTest.Models;
using Gluh.TechnicalTest.Database;
using Gluh.TechnicalTest.Domain;
using System.Linq;
using Gluh.TechnicalTest.Domain.BruteForce;

namespace Gluh.TechnicalTest
{
    /*
     * Assumptions:
     * 1.   ShippingCostMinOrderValue means what it says, i.e. if MivVal=10, any order below 10 gets free delivery;
     *  In a realworld scenario, I'd get back to product team/BA to double check this as it doesn't make sense
     *  
     * 2.   Availability is preferred over cost, i.e. for a requirement of 10 pieces of product X
     *      Given: 
     *          - Supplier A costs 100 including shipping, and have stock to fulfill requirements
     *          - Supplier B cost 90 including shipping, and doesn't have enough stock
     *      Then a PO is created for supplier A.
     *      
     * 3.  A realworld functionality should consider other aspects, like delivery time, supplier trustworthiness, contract obligations, etc...
     *   And potentially a preferred strategy, of which different factors get different weights.
     *   I'm purposfully neglecting these dimensions because they're not mentioned in the requirements.
     *   
     * 4.  Only POs with at least a single physical product are subject to shipping calculations.
     *   i.e. When a PO has no physical items, shipping fee doesn't apply.
     *   
     * 5.  All prices are assumed to be GST inclusive, so, no tax calculations are performed.
     *   
     * Design principals:
     * 1. This is a simplified design and isn't intended for production, following points elaborates more on this.
     * 2. Instead of using a mapper I opted for a helper class.ideally, I'd either make the Data layer return domain objects, or use a mapper to do the task.
     * 3. I won't perform mapping or create domain objects if they'll be identical to database objects, e.g. Supplier; 
     * this breaks DDD as it mixes between domain and Data layers, but I'm doing it for simplicity.
     */
    public class PurchaseOptimizer
    {
        /// <summary>
        /// Calculates the optimal set of supplier to purchase products from.
        /// ### Complete this method
        /// </summary>
        public void Optimize(List<PurchaseRequirement> purchaseRequirements)
        {
            var batch = new RequirementBatch(purchaseRequirements);
            var processed = Filter(purchaseRequirements, out var purchaseOrder, out var unfulfilledOrder);
            foreach (var req in processed)
            {
                purchaseRequirements.Remove(req);
            }

            var requirements = purchaseRequirements.Select(x=>GetAllPremutations(x)).Where(x=>x.Length>0).ToArray();

            var premuter = new Premuting<PurchaseOrderLine[]>(requirements);
            var progress = new ProgressBar();
            premuter.Premute((x, y, z) => ProcessPo(x, y, z, progress));
            progress.Dispose();
            Console.WriteLine($"MaxVal: {maxVal:c} MinVal: {minVal:c}");
           
        }

        /// <summary>
        /// Removes requirements with zero stock, and fulfills requirements from cheapest vendors with zero shipping
        /// </summary>
        /// <param name="requirements"></param>
        /// <returns></returns>
        private List<PurchaseRequirement> Filter(List<PurchaseRequirement> requirements, 
            out Dictionary<Supplier, IPurchaseOrder> purchaseOrders, out IUnfulfilledOrder unfulfilledOrder)
        {
            purchaseOrders = new Dictionary<Supplier,IPurchaseOrder>();
            unfulfilledOrder = new UnfulfilledOrder();
            var processed = new List<PurchaseRequirement>();
            foreach (var req in requirements)
            {
                if (req.Product.Stock.Sum(x => x.StockOnHand) == 0)
                {
                    unfulfilledOrder.Add(new OrderLineBase(req.Product.ToProduct(), req.Quantity));
                    processed.Add(req);
                }
                
                var stocks = req.Product.Stock
                    .Where(x => x.StockOnHand > 0)
                    .OrderByDescending(x => x.Cost);

                var allocated = 0;
                foreach (var stock in stocks)
                {
                    var quantity = Math.Min(req.Quantity - allocated, stock.StockOnHand);
                    var cost = quantity * stock.Cost;
                    if (stock.Supplier.ShippingCost == 0 || stock.Supplier.ShippingCostMaxOrderValue < cost)
                    {
                        allocated += quantity;
                        if (!purchaseOrders.ContainsKey(stock.Supplier))
                        {
                            purchaseOrders.Add(stock.Supplier, new PurchaseOrder(stock.Supplier));
                        }
                        var line = new PurchaseOrderLine(stock.Supplier, stock.Product.ToProduct(), stock.Cost, req.Quantity);
                        purchaseOrders[stock.Supplier].Add(line);
                        if (quantity == allocated) break;
                    }
                    else break;
                }

                req.Quantity -= allocated;
                if (req.Quantity == 0) processed.Add(req);
            }
            return processed;
        }

        private List<PurchaseOrder> maxPo;
        private decimal maxVal = 0;
        private List<PurchaseOrder> minPo;
        private decimal minVal = decimal.MaxValue;
        private void ProcessPo(PurchaseOrderLine[][] lines, long total, long current, ProgressBar progress)
        {
            //var suppliers = lines.SelectMany(x => x).GroupBy(x => x.Supplier).ToDictionary(x => x.Key, y => y.ToList());
            var pos = lines.SelectMany(x => x).GroupBy(x => x.Supplier).Select(x => new PurchaseOrder(x.Key, x.ToList())).ToList();
            var cost = pos.Sum(x => x.Total);
            if(cost> maxVal)
            {
                maxVal = cost;
                maxPo = pos;
            }
            if (cost < minVal)
            {
                minVal = cost;
                minPo = pos;
            }
            progress.Report(current / total);
        }

        private PurchaseOrderLine[][] GetAllPremutations(PurchaseRequirement requirement)
        {
            var vendors = new PurchaseOrderLine[requirement.Product.Stock.Count][];
            var maxAvailable = 0;
            for (int i = 0; i < requirement.Product.Stock.Count; i++)
            {
                vendors[i] = GetLines(requirement.Quantity, requirement.Product.Stock[i]);
                maxAvailable += requirement.Product.Stock[i].StockOnHand;
            }
            if (maxAvailable == 0) return new PurchaseOrderLine[0][];
            var premuter = new Premuting<PurchaseOrderLine>(vendors);
            var result = new List<PurchaseOrderLine[]>();
            var target = Math.Min(maxAvailable, requirement.Quantity);
            premuter.Premute((x,y,z) =>
            {
                if (IsValidCombination(x, target)) result.Add(x.Where(y=>y.Quantity>0).ToArray());
            });
            return result.ToArray();
        }

        private bool IsValidCombination(PurchaseOrderLine[] combination, int target)
        {
            return combination.Sum(x => x.Quantity) == target;
        }

        private PurchaseOrderLine[] GetLines(int required, ProductStock stock)
        {
            var target = Math.Min(required, stock.StockOnHand);
            var result = new PurchaseOrderLine[target+1];
            for (int i = 0; i <= target; i++)
            {
                result[i] = new PurchaseOrderLine(stock.Supplier, stock.Product.ToProduct(), stock.Cost, i);
            }
            return result;
        }
    }
}
