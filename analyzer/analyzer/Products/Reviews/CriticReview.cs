using System;

namespace analyzer.Products.Reviews
{
    public class CriticReview : Review
    {
        public CriticReview(int id, bool isCriticReview, double rating, double maxRating, DateTime date, string title, string url, string category, string content, string author) 
                    : base(id, isCriticReview, rating, maxRating, date, title, url, category, content, author)
        {
        }
    }
}