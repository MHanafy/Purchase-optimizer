using Gluh.TechnicalTest.Database;
using Gluh.TechnicalTest.Models;
using System.Collections.Generic;
using System.Linq;

namespace Gluh.TechnicalTest.Domain
{
    public interface IStockCache
    {
        /// <summary>
        /// Returns available stock cheapest first.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        IEnumerable<Stock> GetAvailableStock(IProduct product, Supplier supplier = null);
    }

    public class StockCache : IStockCache
    {
        private readonly Dictionary<IProduct,List<Stock>> _stock;
        //Again this is a bad practice, domain should be isolated from data layer.
        public StockCache(List<PurchaseRequirement> requirements)
        {
            _stock = new Dictionary<IProduct, List<Stock>>();
            foreach (var req in requirements)
            {
                var product = req.Product.ToProduct();
                var stockList = req.Product.Stock
                    .Where(x=>x.StockOnHand > 0)
                    .Select(x => new Stock(product, x.Supplier, x.StockOnHand, x.Cost))
                    .ToList();
                _stock.Add(product, stockList);
            }
        }

        public IEnumerable<Stock> GetAvailableStock(IProduct product, Supplier supplier = null)
        {
            if (!_stock.ContainsKey(product)) return Enumerable.Empty<Stock>();
            var stock = supplier == null 
                ? _stock[product].Where(x => x.Quantity > 0)
                : _stock[product].Where(x => x.Supplier == supplier && x.Quantity > 0);
            return stock.OrderBy(x => x.Cost);
        }
    }
}
