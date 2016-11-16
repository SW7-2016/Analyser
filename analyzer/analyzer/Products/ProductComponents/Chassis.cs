using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    public class Chassis : ComputerComponents
    {
        public Chassis(string category, int id, string type, bool atx, bool miniAtx, bool miniItx,
                    string fans, string brand, string height, string width, string depth, string weight)
            : base(id, category)
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
    }
}
