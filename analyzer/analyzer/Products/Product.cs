using System.Collections.Generic;
using analyzer.Products.Reviews;
using analyzer.Products.Retailers;
using System.Text.RegularExpressions;

namespace analyzer.Products
{
    public abstract class Product
    {
        public double superScore = 0;
        public double criticScore = 0;
        public double userScore = 0;
        public string name = "";
        public string description = "";
        public List<Retailer> retailers = new List<Retailer>();

        protected Product(int id, string category)
        {
            Id = id;
            Category = category;
        }

        public string Category { get; }
        public int Id { get; }
    }
}
