using System.Collections.Generic;
using System.Diagnostics;
using analyzer.Products;
using analyzer.Products.Reviews;

namespace analyzer.CompareAndMerge
{
    class Compare
    {
        public List<ProductReviewId> MatchReviewAndProduct<T>(List<T> productList, List<CriticReview> reviewList, List<ProductReviewId> productReviewIds ) where T : Product
        {
            foreach (var product in productList)
            {
                foreach (var review in reviewList)
                {
                    if (CompareTokens(product.TokenList, review.TokenList) > 80)
                    {
                        productReviewIds.Add(new ProductReviewId(product.Id, review.Id)); 
                    }
                }
            }
            return productReviewIds;
        }

        private int CompareTokens(List<string> productTokenList, List<string> reviewTokenList)
        {
            

            return 0;
        }
    }
}
