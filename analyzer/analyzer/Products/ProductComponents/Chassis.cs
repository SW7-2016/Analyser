using System.Collections.Generic;
using analyzer.Products.Reviews;
using MySql.Data.MySqlClient;
using analyzer.Products.DistinctProductList;

namespace analyzer.Products.ProductComponents
{
    public class Chassis : ComputerComponents
    {
        public Chassis(string category, int id, string name, string type, bool atx, bool miniAtx, bool miniItx,
                    string fans, string brand, string height, string width, string depth, string weight)
            : base(id, category, name)
        {
            Type = type;
            Atx = atx;
            MiniAtx = miniAtx;
            MiniItx = miniItx;
            Fans = fans;
            Brand = brand;
            Height = height;
            Width = width;
            Depth = depth;
            Weight = weight;
        }

        public bool Atx { get; }
        public bool MiniAtx { get; }
        public bool MiniItx { get; }
        public string Type { get; }
        public string Brand { get; }
        public string Height { get; }
        public string Width { get; }
        public string Weight { get; }
        public string Depth { get; }
        public string Fans { get; }

        public override void WriteToDB(MySqlConnection dbConnection)
        {
            throw new System.NotImplementedException();
        }
    }
}
