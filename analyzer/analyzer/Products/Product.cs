using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using analyzer.Products.Retailers;
using analyzer.Products.Reviews;
using analyzer.Products.ProductComponents;
using analyzer.GetRawData;
using MySql.Data.MySqlClient;
using analyzer.Products.DistinctProductList;
using analyzer.Score;

namespace analyzer.Products
{
    public abstract class Product
    {
        public List<int> prunNumbers= new List<int>();
        public bool scoreAssessed = false;
        public int superScore = 0;
        public int criticScore = 0;
        public int userScore = 0;
        public DateTime oldestReviewDate;
        public DateTime newestReviewDate;
        public string description = "";
        public List<Retailer> retailers;
        public List<Review> reviewMatches;
        public Score2 score = new Score2();
        public double productFactor = 4;

        protected Product(int id, string category, string name)
        {
            Id = id;
            Category = category;
            Name = name;
            retailers = new List<Retailer>();
            reviewMatches = new List<Review>();
        }

        public string Category { get; }
        public string Name { get; }
        public int Id { get; }

        internal List<string> SplitStringToTokens(string name)
        {
            List<string> tokenList;
            Regex rgx = new Regex(@"(\s)|([\-])|(\,)|(\.)|(\()|(\))|(\/)");
            string result = rgx.Replace(name, " ").ToLower();

            tokenList = result.Split(' ').ToList();
            tokenList.RemoveAll(item => item == "");
            tokenList = tokenList.OrderByDescending(item => item.Length).ToList();

            return tokenList;
        }

        internal List<string> RemoveRestrictedTokens(List<string> stringToProcess, Dictionary<string, bool> restrictedTokens)
        {
            foreach (var token in restrictedTokens)
            {
                stringToProcess.RemoveAll(t => t.Equals(token.Key));
            }

            return stringToProcess;
        }
        
       public abstract void MatchReviewAndProduct(DistinctReviewList<Review> reviewList, Dictionary<string, bool> stopWords, ref ReviewProductLinks reviewProductLinks);

        internal virtual bool MatchReviewTitleWithProductStrings(string concatenatedReviewTitle, List<string> allTokens)
        {
            List<string> nonDuplicateAllTokens = new List<string>();
            int count = 0;

            foreach (var token in allTokens)
            {
                if (!nonDuplicateAllTokens.Contains(token))
                {
                    nonDuplicateAllTokens.Add(token);
                }
            }

            nonDuplicateAllTokens = nonDuplicateAllTokens.OrderByDescending(item => item.Length).ToList();

            foreach (var token in nonDuplicateAllTokens)
            {
                if (concatenatedReviewTitle.Contains(token))
                {
                    concatenatedReviewTitle = concatenatedReviewTitle.Replace(token, "");
                    count++;
                }
            }

            if (count == nonDuplicateAllTokens.Count)
            {
                return true;
            }

            return false;
        }

        public virtual void MatchReviewAndProduct1(DistinctReviewList<Review> reviewList, Dictionary<string, bool> stopWords, ref ReviewProductLinks reviewProductLinks)
        {
        }

        internal virtual bool CompareReviewTitleWithProductStrings1(string concatenatedReviewTitle, List<string> allTokens, Dictionary<string, bool> stopWords)
        {
            foreach (string newToken in allTokens)
            {
                if (!SplitStringToTokens(concatenatedReviewTitle).Contains(newToken) && newToken != "" &&
                    !stopWords.ContainsKey(newToken))
                {
                    return false;
                }
            }

            return true;
        }

        public abstract void WriteToDB(MySqlConnection dbConnection);
    }
}

