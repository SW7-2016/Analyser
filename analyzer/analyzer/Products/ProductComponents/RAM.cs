using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    public class RAM : ComputerComponents
    {
        public RAM(int id, string category, string type, string capacity, string speed, 
                    string technology, string formFactor, string casLatency) 
            : base(id, category)
        {
            Type = type;
            Capacity = capacity;
            Speed = speed;
            Technology = technology;
            FormFactor = formFactor;
            CasLatency = casLatency;
        }

        public string Type { get; }
        public string Capacity { get; }
        public string Speed { get; }
        public string Technology { get; }
        public string FormFactor { get; }
        public string CasLatency { get; }
    }
}
