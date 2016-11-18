using System.Collections.Generic;
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
    }
}
