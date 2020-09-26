﻿using System.Collections.Generic;
using Gluh.TechnicalTest.Models;
using Gluh.TechnicalTest.Domain;
using System.Linq;
using Gluh.TechnicalTest.Domain.Allocators;

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

            foreach (var allocator in _allocators)
            {
                allocator.Allocate(batch);
            }
        }

        private readonly List<IAllocator> _allocators;

        public PurchaseOptimizer()
        {
            _allocators = GetAllocators().OrderBy(x => x.Priority).ToList();
        }

        protected virtual IEnumerable<IAllocator> GetAllocators()
        {
            return new List<IAllocator> {new NoShippingAllocator(), new AmendingAllocator(), new BestCombinationAllocator() };
        }


    }
}
