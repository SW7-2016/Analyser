using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    public class PSU : ComputerComponents
    {
        public PSU(int id, string category, string power, string formFactor, bool modular, string width, 
                    string depth, string height, string weight, string brand) 
            : base(id, category)
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
    }
}
