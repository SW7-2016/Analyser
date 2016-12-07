using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using analyzer.Products;
using analyzer.Products.DistinctProductList.types;
using analyzer.Products.Reviews;

namespace analyzer.Threading
{
    class ThreadingData<T>
    {
        public int id;
        public DistinctProductList<T> productList;
        public List<Review> reviewList;

        public ThreadingData(int Id, DistinctProductList<T> ProductList, List<Review> ReviewList)
        {
            id = Id;
            productList = ProductList;
            reviewList = ReviewList;
        }
    }
}
