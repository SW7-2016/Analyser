using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.CompareAndMerge
{
    class ProductReviewId
    {
        private int productID;
        private int reviewID;

        public ProductReviewId(int productID, int reviewID)
        {
            this.reviewID = reviewID;
            this.productID = productID;
        }
    }
}
