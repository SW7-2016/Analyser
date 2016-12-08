using System.Collections.Generic;
using analyzer.Products.Reviews;
using analyzer.Products.DistinctProductList;

namespace analyzer.Products
{
    public abstract class ComputerComponents : Product
    {
        protected ComputerComponents(int id, string category, string name) : base(id, category, name)
        {
        }

        public override void MatchReviewAndProduct(DistinctReviewList<Review> reviewList, Dictionary<string, bool> stopWords, ref ReviewProductLinks reviewProductLinks)
        {
            throw new System.NotImplementedException();
        }
    }
}
