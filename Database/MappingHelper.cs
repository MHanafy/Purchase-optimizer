﻿using Gluh.TechnicalTest.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gluh.TechnicalTest.Database
{
    public static class MappingHelper
    {
        public static IProduct ToProduct(this Product product, decimal cost)
        {
            return new Domain.Product(product.ID, product.Name, cost, product.Type);
        }
    }
}
