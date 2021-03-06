﻿using System;

namespace analyzer.Products.Reviews
{
    public class UserReview : Review
    {
        public UserReview(int id, bool isCriticReview, double rating, double maxRating, DateTime date, string title, string url, string category, bool verifiedPurchase, string content, string author)
                    : base(id, isCriticReview, rating, maxRating, date, title, url, category, content, author)
        {
            VerifiedPurchase = verifiedPurchase;
        }

        public bool VerifiedPurchase { get; }
    }
}