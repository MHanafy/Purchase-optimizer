using Gluh.TechnicalTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gluh.TechnicalTest.Domain
{


    /*
                     if (req.Product.Stock.Sum(x => x.StockOnHand) == 0)
            {
                unfulfilledOrder.Add(new OrderLineBase(req.Product.ToProduct(0), req.Quantity));
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
                    var line = new PurchaseOrderLine(stock.Supplier, stock.Product.ToProduct(stock.Cost), req.Quantity);
                    purchaseOrders[stock.Supplier].Add(line);
                    if (quantity == allocated) break;
                }
                else break;
            }

            req.Quantity -= allocated;
            if (req.Quantity == 0) processed.Add(req);
     */
    public interface IStockCache
    {
        /// <summary>
        /// Returns available stock in descending order based on cost.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        IEnumerable<ProductStock> GetAvailableStock(Product product);
    }

    public class StockCache
    {
        private readonly Dictionary<Product, List<ProductStock>> _stock;
        //Again this is a bad practice, domain should be isolated from data layer.
        public StockCache(List<PurchaseRequirement> requirements)
        {
            _stock = new Dictionary<Product, List<ProductStock>>();
            foreach (var req in requirements)
            {

            }
        }
    }
}
