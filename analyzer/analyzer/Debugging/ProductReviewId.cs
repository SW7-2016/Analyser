using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.CompareAndMerge
{
    class ProductReviewId
    {
        private List<int> productID;
        private int reviewID;

        public ProductReviewId(List<int> productId, int reviewId)
        {
            productID = productId;
            reviewID = reviewId;
        }

        public List<int> ProductId
        {
            get { return productID; }
            set { productID = value; }
        }

        public int ReviewId
        {
            get { return reviewID; }
        }

        public override string ToString()
        {
            return $"{nameof(productID)}: {productID}, {nameof(reviewID)}: {reviewID}";
        }
    }
}
