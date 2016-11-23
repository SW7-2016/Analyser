using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace analyzer.Products.Reviews
{
    public abstract class Review
    {
        public List<string> pros;
        public List<string> cons;

        public int productId;
        public int positiveReception;
        public int negativeReception;
        public double credibility;
        public double timeDecayWeight;

        protected Review(int id, double rating, double maxRating, DateTime date, string title, string url, string category)
        {
            Id = id;
            Category = category;
            Url = url;
            Title = title;
            ReviewDate = date;
            Rating = rating;
            MaxRating = maxRating;
        }

        public int Id { get; }
        public double Rating { get; }
        public double MaxRating { get; }
        public DateTime ReviewDate { get; }
        public string Url { get; }
        public string Title { get; }
        public string Category { get; }

        public List<string> TokenList
        {
            get { return SplitTitle(Title); }
        }

        private List<string> SplitTitle(string name)
        {
            List<string> tokenNameList = new List<string>();

            Regex rgx = new Regex(@"(\s)|([\-])|(\,)|(\.)|(\()|(\))|(\/)");
            string result = rgx.Replace(name, " ");

            tokenNameList = result.Split(' ').ToList();
            tokenNameList.RemoveAll(item => item == "");

            return tokenNameList;
        }
    }
}