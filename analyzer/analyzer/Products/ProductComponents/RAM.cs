using System.Collections.Generic;
using analyzer.Products.Reviews;

namespace analyzer.Products.ProductComponents
{
    public class RAM : ComputerComponents
    {
        public RAM(string category, int id, string name, string type, string capacity, string speed, 
                    string technology, string formFactor, string casLatency) 
            : base(id, category, name)
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
        public override void MatchReviewAndProduct<T>(List<Review> reviewList, List<T> productList)
        {
            throw new System.NotImplementedException();
        }
    }
}
