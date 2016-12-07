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
        public List<Product> linkedProducts;

        public int positiveReception;
        public int negativeReception;
        public double productAgeAtReviewTime;
        public double credibility;
        public double reviewWeight;
        public double normalizedScore;
        public bool isCritic = false;
        private string content = "";//todo must be set
        private string author = "";//todo must be set



        protected Review(int id, double rating, double maxRating, DateTime date, string title, string url, string category)
        {
            Id = id;
            Category = category;
            Url = url;
            Title = title;
            ReviewDate = date;
            Rating = rating;
            MaxRating = maxRating;
            linkedProducts = new List<Product>();
        }

        public int Id { get; }
        public double Rating { get; }
        public double MaxRating { get; }
        public DateTime ReviewDate { get; }
        public string Url { get; }
        public string Title { get; }
        public string Category { get; }
        public string Content { get; }
        public string Author { get; }

        public List<string> TokenList
        {
            get { return SplitTitle(Title); }
        }

        internal List<string> SplitTitle(string name)
        {
            List<string> tokenNameList = new List<string>();

            Regex rgx = new Regex(@"(\s)|([\-])|(\,)|(\.)|(\()|(\))|(\/)");
            string result = rgx.Replace(name, " ").ToLower();

            tokenNameList = result.Split(' ').ToList();
            tokenNameList.RemoveAll(item => item == "");

            return tokenNameList;
        }

        internal MatchCollection ExtractNumbersFromString(string str)
        {
            MatchCollection result = Regex.Matches(str, @"\d+");
            return result;
        }


        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Rating)}: {Rating}, {nameof(ReviewDate)}: {ReviewDate}, {nameof(Url)}: {Url}, {nameof(Title)}: {Title}, {nameof(Category)}: {Category}";
        }
    }
}