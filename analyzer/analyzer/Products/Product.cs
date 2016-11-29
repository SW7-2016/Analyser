using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using analyzer.Products.Retailers;
using analyzer.Products.Reviews;
using analyzer.Products.ProductComponents;

namespace analyzer.Products
{
    public abstract class Product
    {
        public double superScore = 0;
        public double criticScore = 0;
        public double userScore = 0;
        public string description = "";
        public List<Retailer> retailers = new List<Retailer>();
        public List<int> reviewMatches = new List<int>();
        public List<KeyValuePair<int, string>> reviewMatche = new List<KeyValuePair<int, string>>();
        public List<KeyValuePair<int, string>> reviewMatcheConcatenate = new List<KeyValuePair<int, string>>();

        protected Product(int id, string category, string name)
        {
            Id = id;
            Category = category;
            Name = name;
        }

        public string Category { get; }
        public string Name { get; }
        public int Id { get; }
        public List<String> TokenList
        {
            get { return SplitStringToTokens(Name); }
        }

        internal List<string> SplitStringToTokens(string name)
        {
            List<string> tokenNameList = new List<string>();

            Regex rgx = new Regex(@"(\s)|([\-])|(\,)|(\.)|(\()|(\))|(\/)");
            string result = rgx.Replace(name, " ").ToLower();

            tokenNameList = result.Split(' ').ToList();
            tokenNameList.RemoveAll(item => item == "");
            tokenNameList = tokenNameList.OrderByDescending(item => item.Length).ToList();

            return tokenNameList;
        }

        internal string ConcatenateString(string name)
        {
            Regex rgx = new Regex(@"(\s)|([\-])|(\,)|(\.)|(\()|(\))|(\/)");
            string result = rgx.Replace(name, "").ToLower();

            return result;
        }

        internal MatchCollection ExtractNumbersFromString(string str)
        {
            MatchCollection result = Regex.Matches(str, @"\d+");
            return result;
        }

        public abstract void MatchReviewAndProduct<T>(List<Review> reviewList, List<T> productList) where T : Product;

        public virtual void MatchReviewAndProduct2<T>(List<Review> reviewList, List<T> productList) where T : Product
        {
        }
    }
}
