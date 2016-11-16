using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    public class HardDrive : ComputerComponents
    {
        public HardDrive(int id, string category, bool isInternal, string type, string formFactor, string capacity, string cacheSize, 
                        string transferRate, string brand, string sata, string height, string depth, string width) 
            : base(id, category)
        {
            IsInternal = isInternal;
            Type = type;
            FormFactor = formFactor;
            Capacity = capacity;
            CacheSize = cacheSize;
            TransferRate = transferRate;
            Brand = brand;
            Sata = sata;
            Height = height;
            Depth = depth;
            Width = width;
        }

        public bool IsInternal { get; }
        public string Type { get; }
        public string FormFactor { get; }
        public string Capacity { get; }
        public string CacheSize { get; }
        public string TransferRate { get; }
        public string Brand { get; }
        public string Sata { get; }
        public string Height { get; }
        public string Depth { get; }
        public string Width { get; }
    }
}
