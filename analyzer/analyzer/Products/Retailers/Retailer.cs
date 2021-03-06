﻿namespace analyzer.Products.Retailers
{
    public class Retailer
    {
        public Retailer(int id, decimal price, string name, string url)
        {
            Id = id;
            Price = price;
            Name = name;
            Url = url;
        }

        public int Id { get; }
        public decimal Price { get; }
        public string Name { get; }
        public string Url { get; }
    }
}
