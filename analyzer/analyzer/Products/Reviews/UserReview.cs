using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.Products.Reviews
{
    public class UserReview : Review
    {
        public UserReview(int id, int productId, double rating, double maxRating, DateTime date, string title,
            string url, string category, bool verifiedPurchase)
            : base(id, productId, rating, maxRating, date, title, url, category)
        {
            VerifiedPurchase = verifiedPurchase;
        }

        public bool VerifiedPurchase { get; }
    }
}