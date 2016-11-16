using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.Products.Reviews
{
    public abstract class Review
    {
        public List<string> pros;
        public List<string> cons;

        public int positiveReception;
        public int negativeReception;
        public double credibility;
        public double timeDecayWeight;

        protected Review(int id, int productId, double rating, double maxRating, DateTime date, string title,  string url, string category)
        {
            Id = id;
            ProductId = productId;
            Category = category;
            Url = url;
            Title = title;
            ReviewDate = date;
            Rating = rating;
            MaxRating = maxRating;
        }

        public int Id { get; }
        public int ProductId { get; }
        public double Rating { get; }
        public double MaxRating { get; }
        public DateTime ReviewDate { get; }
        public string Url { get; }
        public string Title { get; }
        public string Category { get; }
    }
}