using System;
using System.Collections.Generic;
using System.Text;
using Gluh.TechnicalTest.Models;
using Gluh.TechnicalTest.Database;

namespace Gluh.TechnicalTest
{
    /*
     * Assumptions:
     * 1.   ShippingCostMinOrderValue means what it says, i.e. if MivVal=10, any order below 10 gets free delivery;
     *  In a realworld scenario, I'd get back to product team/BA to double check this as it doesn't make sense
     * 2.   Availability is preferred over cost, i.e. for a requirement of 10 pieces of product X
     *      Given: 
     *          - Supplier A costs 100 including shipping, and have stock to fulfill requirements
     *          - Supplier B cost 90 including shipping, and doesn't have enough stock
     *      Then a PO is created for supplier A.
     *  3.  A realworld functionality should consider other aspects, like delivery time, supplier trustworthiness, contract obligations, etc...
     *      And potentially a preferred strategy, of which different factors get different weights.
     *      I'm puposfully neglecting these dimensions because they're not mentioned in the requirements.
     */
    public class PurchaseOptimizer
    {
        /// <summary>
        /// Calculates the optimal set of supplier to purchase products from.
        /// ### Complete this method
        /// </summary>
        public void Optimize(List<PurchaseRequirement> purchaseRequirements)
        {
            
        }
    }
}
