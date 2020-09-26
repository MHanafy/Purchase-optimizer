using Gluh.TechnicalTest.Domain.BruteForce;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gluh.TechnicalTest.Domain.Allocators
{
    public class BestCombinationAllocator : AllocatorBase
    {
        public BestCombinationAllocator(int priority = 90) : base(priority)
        {
        }

        protected override void AllocateAll(IRequirementBatch batch)
        {
            Reset();
            var requirements = batch.Unallocated
                .Select(x => GetAllPremutations(x, batch.Cache.GetAvailableStock(x.Product).ToList()))
                .ToArray();

            var premuter = new Premuting<PurchaseOrderLine[]>(requirements);
            var progress = new ProgressBar();
            premuter.Premute((combination, y, z) => ProcessPo(batch, combination, y, z, progress));
            foreach (var item in _cheapest)
            {
                batch.AddPurchaseOrderLine(item);
            };
            progress.Dispose();
        }

        private void Reset()
        {
            _cheapest = null;
            _lowestTotal = decimal.MaxValue;
            _mostExpensive = null;
            _highestTotal = decimal.MinValue;
        }

        private IEnumerable<IPurchaseOrderLine> _cheapest;
        private decimal _lowestTotal;

        private IEnumerable<IPurchaseOrderLine> _mostExpensive;
        private decimal _highestTotal;
        private void ProcessPo(IRequirementBatch batch, PurchaseOrderLine[][] lines, long total, long current, ProgressBar progress)
        {
            var supplierLines = lines.SelectMany(x => x);
            var cost = batch.CalculateEffectiveCost(supplierLines);
            if (cost > _highestTotal)
            {
                _highestTotal = cost;
                _mostExpensive = supplierLines;
            }
            if(cost< _lowestTotal)
            {
                _cheapest = supplierLines;
                _lowestTotal = cost;
            }
            progress.Report(current / total);
        }

        private PurchaseOrderLine[][] GetAllPremutations(IRequirement requirement, IList<Stock> stocks)
        {
            var vendors = new PurchaseOrderLine[stocks.Count][];
            var maxAvailable = 0;
            for (int i = 0; i < stocks.Count; i++)
            {
                vendors[i] = GetLines(requirement.Quantity, stocks[i]);
                maxAvailable += stocks[i].Quantity;
            }
            if (maxAvailable == 0) return new PurchaseOrderLine[0][];
            var premuter = new Premuting<PurchaseOrderLine>(vendors);
            var result = new List<PurchaseOrderLine[]>();
            var target = Math.Min(maxAvailable, requirement.Quantity);
            premuter.Premute((x, y, z) =>
            {
                if (IsValidCombination(x, target)) result.Add(x.Where(y => y.Quantity > 0).ToArray());
            });
            return result.ToArray();
        }

        private bool IsValidCombination(PurchaseOrderLine[] combination, int target)
        {
            return combination.Sum(x => x.Quantity) == target;
        }

        private PurchaseOrderLine[] GetLines(int required, Stock stock)
        {
            var target = Math.Min(required, stock.Quantity);
            var result = new PurchaseOrderLine[target + 1];
            for (int i = 0; i <= target; i++)
            {
                result[i] = new PurchaseOrderLine(stock.Supplier, stock.Product, stock.Cost, i);
            }
            return result;
        }
    }
}
