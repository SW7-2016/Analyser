using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
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

        public override void MatchReviewAndProduct<T>(List<Review> reviewList, List<T> productList)
        {
            bool manufactureMatch = false; //Must match
            bool modelMatch = false; //Number must match
            bool CPUSeriesMatch = false; //Must match

            //Split model and cpuSeries into tokens
            List<string> modelToken = SplitStringToTokens(Model.ToLower());
            List<string> cpuSeriesToken = SplitStringToTokens(CpuSeries.ToLower());

            foreach (var review in reviewList)
            {
                foreach (string token in review.TokenList)
                {
                    if (token == Manufacturer.ToLower())
                    {
                        manufactureMatch = true;
                    }

                    //Only match if every word in model is pressent in review title
                    int tempCount = 0;
                    foreach (string model in modelToken)
                    {
                        if (token == model)
                            tempCount++;

                        if (tempCount == modelToken.Count)
                        {
                                modelMatch = true;
                        }
                    }

                    foreach (string series in cpuSeriesToken)
                    {
                        if (token == series)
                            CPUSeriesMatch = true;
                    }

                }

                if (manufactureMatch && modelMatch && CPUSeriesMatch)
                {
                    reviewMatches.Add(review.Id);
                    Debug.WriteLine(review.ToString());
                    Debug.WriteLine(this.ToString());
                    Debug.WriteLine("");
                }

                manufactureMatch = false;
                modelMatch = false;
                CPUSeriesMatch = false;
            }
        }


        private bool CompareCPUModel(string reviewTitle, string model)
        {
            MatchCollection GPUprocessorNumbers = ExtractNumbersFromString(model);
            MatchCollection reviewTitleNumbers = ExtractNumbersFromString(reviewTitle);
            int GPUCount = 0;
            int reviewCount = 0;

            foreach (Match GPUnumber in GPUprocessorNumbers)
            {
                foreach (Match reviewNumber in reviewTitleNumbers)
                {
                    if (int.Parse(GPUnumber.Value) == int.Parse(reviewNumber.Value))
                    {
                        return true;
                    }
                    reviewCount++;
                }
                GPUCount++;
            }
            return false;
        }

        public override string ToString()
        {
            return $"{nameof(PhysicalCores)}: {PhysicalCores}, {nameof(LogicalCores)}: {LogicalCores}, {nameof(StockCooler)}: {StockCooler}, {nameof(Model)}: {Model}, {nameof(Clock)}: {Clock}, {nameof(Socket)}: {Socket}, {nameof(MaxTurbo)}: {MaxTurbo}, {nameof(IntegratedGpu)}: {IntegratedGpu}, {nameof(Manufacturer)}: {Manufacturer}, {nameof(CpuSeries)}: {CpuSeries}";
        }
    }

}