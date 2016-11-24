using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using analyzer.Products.Retailers;

namespace analyzer.Products
{
    public abstract class Product
    {
        public double superScore = 0;
        public double criticScore = 0;
        public double userScore = 0;
        public string description = "";
        public List<Retailer> retailers = new List<Retailer>();

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
            get { return SplitName(Name); }
        }

        private List<string> SplitName(string name)
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
