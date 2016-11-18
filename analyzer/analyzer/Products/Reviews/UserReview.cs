﻿using System;

namespace analyzer.Products.Reviews
{
    public class UserReview : Review
    {
        public UserReview(int id, double rating, double maxRating, DateTime date, string title, string url, string category, bool verifiedPurchase)
                    : base(id, rating, maxRating, date, title, url, category)
        {
            VerifiedPurchase = verifiedPurchase;
        }

        public bool VerifiedPurchase { get; }
    }
}