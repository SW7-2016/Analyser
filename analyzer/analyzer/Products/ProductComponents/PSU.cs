using System.Collections.Generic;
using analyzer.Products.Reviews;
using MySql.Data.MySqlClient;

namespace analyzer.Products.ProductComponents
{
    public class PSU : ComputerComponents
    {
        public PSU(string category, int id, string name, string power, string formFactor, bool modular, string width, 
                    string depth, string height, string weight, string brand) 
            : base(id, category, name)
        {
            Power = power;
            FormFactor = formFactor;
            Modular = modular;
            Width = width;
            Depth = depth;
            Height = height;
            Weight = weight;
            Brand = brand;
        }

        public string Power { get; }
        public string FormFactor { get; }
        public string Brand { get; }
        public string Height { get; }
        public string Depth { get; }
        public string Width { get; }
        public string Weight { get; }
        public bool Modular { get; }
        public override void MatchReviewAndProduct(List<Review> reviewList, Dictionary<string, bool> stopWords, ref ReviewProductLinks reviewProductLinks)
        {
            throw new System.NotImplementedException();
        }

        public override void WriteToDB(MySqlConnection dbConnection)
        {
            throw new System.NotImplementedException();
        }
    }
}
