﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using analyzer.Products.Retailers;
using analyzer.Products.Reviews;
using analyzer.Products.ProductComponents;
using analyzer.GetRawData;
using MySql.Data.MySqlClient;
using analyzer.Products.DistinctProductList;
using analyzer.Scores;

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
        public Score score = new Score();
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

        //splits/tokenizes a string into tokens without empty tokens, and returns the tokens.
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

        //removes stop word tokens from a token list
        internal List<string> RemoveRestrictedTokens(List<string> stringToProcess, Dictionary<string, bool> restrictedTokens)
        {
            foreach (var token in restrictedTokens)
            {
                stringToProcess.RemoveAll(t => t.Equals(token.Key));
            }

            return stringToProcess;
        }

        //
        internal virtual bool MatchReviewTitleWithProductStringsSubstring(string concatenatedReviewTitle, List<string> allTokens)
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

        public virtual void MatchReviewAndProductSubstring(DistinctReviewList<Review> reviewList, Dictionary<string, bool> stopWords, ref ReviewProductLinks reviewProductLinks)
        {
        }

        public virtual void MatchReviewAndProductTokens(DistinctReviewList<Review> reviewList, Dictionary<string, bool> stopWords, ref ReviewProductLinks reviewProductLinks)
        {
        }

        internal virtual bool CompareReviewTitleWithProductStringsToken(string concatenatedReviewTitle, List<string> allTokens, Dictionary<string, bool> stopWords)
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

