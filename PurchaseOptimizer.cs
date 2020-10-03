using System.Collections.Generic;
using Gluh.TechnicalTest.Models;
using Gluh.TechnicalTest.Domain;
using System.Linq;
using Gluh.TechnicalTest.Domain.Allocators;
using System.Text;
using System;
using Gluh.TechnicalTest.UI;

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
     * 1. This is simplified in some areas, and isn't intended for production, following points elaborates more on this.
     * 2. Instead of using a mapper I opted for a helper class.ideally, I'd either make the Data layer return domain objects, or use a mapper to do the task.
     * 3. I won't perform mapping or create domain objects if they'll be identical to database objects, e.g. Supplier; 
     * this breaks DDD as it mixes between domain and Data layers, but I'm doing it for simplicity.
     * 4. Ideally, I would create separate projects for better code organization and loose coupling; I didn't do that
     * 5. I used console.WriteLine instead of a proper logging framework, and didn't use a DI utility.
     * 
     * Testing:
     * 1. I added minimal unit tests just to show how I test; production code should have much more coverage.
     * 2. My preference is AAA style, and following the test pyramid (vast majority of unit tests, minimal integration, and hopefully no UI :) )
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
            StringBuilder summary = new StringBuilder();
            _progress.Show();
            foreach (var allocator in _allocators)
            {
                try
                {
                    summary.Append(allocator.Allocate(batch));
                }
                catch (Exception ex)
                {
                    summary.Append(ex);
                }
            }
            _progress.Hide();
            Console.Write($"\r\n{summary}\r\n");
            _orderPrinter.Print(batch.PurchaseOrders);
            _orderPrinter.Print(batch.UnfulfilledOrder);

            Console.ReadLine();
        }

        private readonly List<IAllocator> _allocators;
        private readonly ProgressBar _progress;
        private readonly IOrderPrinter _orderPrinter;
        public PurchaseOptimizer()
        {
            _allocators = GetAllocators().OrderBy(x => x.Priority).ToList();
            foreach (var allocator in _allocators)
            {
                allocator.Progress += Allocator_Progress;
            }
            _progress = new ProgressBar(false);
            _orderPrinter = new OrderPrinter();
        }

        private void Allocator_Progress(object sender, ProgressEventArgs e)
        {
            var cIndex = _allocators.IndexOf((IAllocator)sender);
            float current = 0;
            float total = 0;
            for (int i = 0; i < _allocators.Count; i++)
            {
                total += _allocators[i].Complexity;
                if(i<cIndex) current+= _allocators[i].Complexity; 
            }
            current += _allocators[cIndex].Complexity * e.Percentage;
            _progress.Report(current / total, e.Activity);
        }

        protected virtual IEnumerable<IAllocator> GetAllocators()
        {
            return new List<IAllocator> {new NoShippingAllocator(), new AmendingAllocator(), new BestCombinationAllocator(), new NostockAllocator(1)};
        }


    }
}
