using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters;
using analyzer.Products.Reviews;

namespace analyzer.Products.ProductComponents
{
    public class CPU : ComputerComponents
    {
        public CPU(string category, int id, string name, string model, string clock, string maxTurbo, string integratedGpu,
                    bool stockCooler, string manufacturer, string cpuSeries, int logicalCores, int physicalCores, string socket)
            : base(id, category, name)
        {
            Model = model;
            Clock = clock;
            MaxTurbo = maxTurbo;
            IntegratedGpu = integratedGpu;
            StockCooler = stockCooler;
            Manufacturer = manufacturer;
            CpuSeries = cpuSeries;
            LogicalCores = logicalCores;
            PhysicalCores = physicalCores;
            Socket = socket;
        }

        public int PhysicalCores { get; }
        public int LogicalCores { get; }
        public bool StockCooler { get; }
        public string Model { get; }
        public string Clock { get; }
        public string Socket { get; }
        public string MaxTurbo { get; }
        public string IntegratedGpu { get; }
        public string Manufacturer { get; }
        public string CpuSeries { get; }

        public override void MatchReviewAndProduct(List<Review> reviewList, ReviewProductLinks reviewProductLinks)
        {
            List<string> restrictedTokens = new List<string>();
            string productStrings = Model.ToLower() + " " + CpuSeries.ToLower();

            restrictedTokens.Add("intel");
            restrictedTokens.Add("core");
            restrictedTokens.Add("amd");
            productStrings = RemoveRestrictedTokens(productStrings, restrictedTokens);

            foreach (var review in reviewList)
            {
                if (review.Category.ToLower() != "cpu")
                    continue;

                string concatenatedReviewTitle = RemoveRestrictedTokens(ConcatenateString(review.Title.ToLower()), restrictedTokens);

                if (!CompareReviewTitleWithProductStrings(concatenatedReviewTitle, productStrings))
                {
                    continue;
                }



                Debug.WriteLine(this.Id + " " + this.ToString());
                Debug.WriteLine(review.Id + " " + review.Title);
                //add review id to product
                reviewMatches.Add(review.Id);
                review.linkedProducts.Add(this.Id);

                if (!reviewProductLinks.productList.Contains(this))
                {
                    reviewProductLinks.productList.Add(this);
                }

                if (!reviewProductLinks.reviewList.Contains(review))
                {
                    reviewProductLinks.reviewList.Add(review);
                }
            }
        }

        /*
        public override void MatchReviewAndProduct<T>(List<Review> reviewList, List<T> productList)
        {
            bool modelMatch = false; //Number must match
            bool CPUSeriesMatch = false; //Must match

            //Split model and cpuSeries into tokens
            List<string> modelToken = SplitStringToTokens(Model.ToLower());
            List<string> cpuSeriesToken = SplitStringToTokens(CpuSeries.ToLower());

            foreach (var review in reviewList)
            {
                if (review.Category != "CPU")
                    continue;
                int tempCount = 0; //used in modle match
                
                foreach (string token in review.TokenList)
                {
                    //Only match if every word in model is pressent in review title
                    foreach (string model in modelToken)
                    {
                        if (token.ToLower() == model.ToLower())
                        {
                            tempCount++;
                        }

                        if (tempCount == modelToken.Count)
                        {
                                modelMatch = true;
                        }
                    }
                    //match cpu series with review
                    foreach (string series in cpuSeriesToken)
                    {
                        
                        if (token == series)
                            CPUSeriesMatch = true;
                        //TODO Fix så den ikke kun får true på den første
                    }
                }

                //if both cpuseries and model is true = link.
                if (modelMatch && CPUSeriesMatch)
                {
                    reviewMatches.Add(review.Id);
                    //Debug.WriteLine(review.ToString());
                    //Debug.WriteLine(this.ToString());
                    //Debug.WriteLine(""); 
                }

                modelMatch = false;
                CPUSeriesMatch = false;
            }
        }*/

        public override string ToString()
        {
            return $"{nameof(PhysicalCores)}: {PhysicalCores}, {nameof(LogicalCores)}: {LogicalCores}, {nameof(StockCooler)}: {StockCooler}, {nameof(Model)}: {Model}, {nameof(Clock)}: {Clock}, {nameof(Socket)}: {Socket}, {nameof(MaxTurbo)}: {MaxTurbo}, {nameof(IntegratedGpu)}: {IntegratedGpu}, {nameof(Manufacturer)}: {Manufacturer}, {nameof(CpuSeries)}: {CpuSeries}";
        }
    }

}