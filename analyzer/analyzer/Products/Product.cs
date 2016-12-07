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

        protected Product(int id, string category, string name)
        {
            Id = id;
            Category = category;
            Name = name;
        }

        public string Category { get; }
        public string Name { get; }
        public int Id { get; }

        internal List<string> SplitStringToTokens(string name)
        {

            List<string> tokenNameList = new List<string>();

            Regex rgx = new Regex(@"(\s)|([\-])|(\,)|(\.)|(\()|(\))|(\/)");
            string result = rgx.Replace(name, " ").ToLower();

            tokenNameList = result.Split(' ').ToList();
            tokenNameList.RemoveAll(item => item == "");
            tokenNameList = tokenNameList.OrderByDescending(item => item.Length).ToList();

            if (name.Contains("-"))
            {

            }

            return tokenNameList;
        }

        internal string ConcatenateString(string name)
        {
            Regex rgx = new Regex(@"(\s)|([\-])|(\,)|(\.)|(\()|(\))|(\/)");
            string result = rgx.Replace(name, "").ToLower();

            return result;
        }

        internal List<string> RemoveRestrictedTokens(List<string> modifyString, Dictionary<string, bool> restrictedTokens)
        {
            foreach (var token in restrictedTokens)
            {
                if (modifyString.Contains(token.Key))
                {
                    modifyString.Remove(token.Key);
                }
            }

            return modifyString;
        }
        
        internal MatchCollection ExtractNumbersFromString(string str)
        {
            MatchCollection result = Regex.Matches(str, @"\d+");
            return result;
        }

        internal bool MatchStringNumbers(string productString1, string reviewString2)
        {
            MatchCollection firstStringNumbers = ExtractNumbersFromString(productString1);
            MatchCollection secondStringNumbers = ExtractNumbersFromString(reviewString2);
            List<bool> correctMatches = new List<bool>();
            bool correctMatch;

            foreach (Match firstStringNumber in firstStringNumbers)
            {
                correctMatch = false;
                foreach (Match secondStringNumber in secondStringNumbers)
                {
                    if (int.Parse(firstStringNumber.Value) == int.Parse(secondStringNumber.Value))
                    {
                        correctMatch = true;
                    }
                }

                if (correctMatch)
                {
                    correctMatches.Add(true);
                }
                else
                {
                    correctMatches.Add(false);
                }
            }

            foreach (var numberMatch in correctMatches)
            {
                if (!numberMatch)
                {
                    return false;
                }
            }

            return true;
        }

        internal bool ReviewLinksToMultipleProducts(Review review)
        {
            if (review.linkedProducts.Count > 1)
            {
                
            }
            return false;
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

        public abstract void MatchReviewAndProduct(List<Review> reviewList, ReviewProductLinks reviewProductLinks, Dictionary<string, bool> stopWords);

        internal virtual bool CompareReviewTitleWithProductStrings(string concatenatedReviewTitle, List<string> allTokens, Dictionary<string, bool> stopWords)
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

            bool isEqual = true;

            foreach (string newToken in allTokens)
            {
                if (!(SplitStringToTokens(concatenatedReviewTitle)).Contains(newToken) && newToken != "" &&
                    !stopWords.ContainsKey(newToken))
                {
                    isEqual = false;
                    break;
                }
                else
                {

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
