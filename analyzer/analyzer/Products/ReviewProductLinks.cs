﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using analyzer.Products.Reviews;

namespace analyzer.Products
{
    public class ReviewProductLinks
    {
        public List<Product> productList;
        public List<Review> reviewList;

        public ReviewProductLinks()
        {
            productList = new List<Product>();
            reviewList = new List<Review>();
        }
    }
}
