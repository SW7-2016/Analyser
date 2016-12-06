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
        public int superScore = 0;
        public double criticScore = 0;
        public double userScore = 0;
        public string description = "";
        public List<Retailer> retailers;
        public List<Review> reviewMatches;

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

        internal string ConcatenateString(string name)
        {
            Regex rgx = new Regex(@"(\s)|([\-])|(\,)|(\.)|(\()|(\))|(\/)");
            string result = rgx.Replace(name, "").ToLower();

            return result;
        }

        internal List<string> RemoveRestrictedTokens(List<string> stringToProcess, Dictionary<string, bool> restrictedTokens)
        {

            foreach (var token in restrictedTokens)
            {
                stringToProcess.RemoveAll(t => t.Equals(token.Key));
            }
            return stringToProcess;
        }
        
        internal bool MatchStringToTokens(string string1, List<string> string2)
        {
            foreach (var token in string2)
            {
                if (string1.ToLower() == token.ToLower())
                {
                    return true;
                }
            }

            return false;
        }

        public abstract void MatchReviewAndProduct(List<Review> reviewList, Dictionary<string, bool> stopWords, ref ReviewProductLinks reviewProductLinks);

        internal virtual bool CompareReviewTitleWithProductStrings(string concatenatedReviewTitle, List<string> allTokens)
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

        public virtual void MatchReviewAndProduct1(List<Review> reviewList, Dictionary<string, bool> stopWords, ref ReviewProductLinks reviewProductLinks)
        {
        }

        internal virtual bool CompareReviewTitleWithProductStrings1(string concatenatedReviewTitle, List<string> allTokens, Dictionary<string, bool> stopWords)
        {
            List<string> nonDuplicateAllTokens = new List<string>();

            foreach (var token in allTokens)
            {
                if (!nonDuplicateAllTokens.Contains(token))
                {
                    nonDuplicateAllTokens.Add(token);
                }
            }
            nonDuplicateAllTokens = nonDuplicateAllTokens.OrderByDescending(item => item.Length).ToList();
            bool isEqual = true;
            foreach (string newToken in allTokens)
            {
                if (!(SplitStringToTokens(concatenatedReviewTitle)).Contains(newToken) && newToken != "" &&
                    !stopWords.ContainsKey(newToken))
                {
                    isEqual = false;
                    break;
                }
            }
            if (isEqual)
            {
                return true;
            }
            return false;
        }
    }
}

