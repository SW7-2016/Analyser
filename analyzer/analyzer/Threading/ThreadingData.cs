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
    static class ThreadingData
    {
        public static int semaphore = 0;
        public static List<ReviewProductLinks> threadProcessedData = new List<ReviewProductLinks>();
    }

    class ThreadingData<T>
    {
        //ThreadingData class is used as an abstraction for the data required by a linking thread
        public int id;
        public DistinctProductList<T> productList; //list of products the thread has to try to link reviews to
        public List<Review> reviewList; //all reviews 
        


        public ThreadingData(int Id, DistinctProductList<T> ProductList, List<Review> ReviewList)
        {
            id = Id;
            productList = ProductList;
            reviewList = ReviewList;
        }
    }
}
