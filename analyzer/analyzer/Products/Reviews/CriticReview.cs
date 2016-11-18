﻿using System;

namespace analyzer.Products.Reviews
{
    public class CriticReview : Review
    {
        public CriticReview(int id, double rating, double maxRating, DateTime date, string title, string url, string category) 
                    : base(id, rating, maxRating, date, title, url, category)
        {
        }
    }
}