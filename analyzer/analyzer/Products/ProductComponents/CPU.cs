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

        public override void MatchReviewAndProduct(List<Review> reviewList, Dictionary<string, bool> stopWords, ref ReviewProductLinks reviewProductLinks)
        {
            List<string> productTokens = SplitStringToTokens(Model.ToLower() + " " + CpuSeries.ToLower());
            productTokens = RemoveRestrictedTokens(productTokens, stopWords);

            foreach (var review in reviewList)
            {
                List<string> reviewTitleWithoutStopWordTokens = RemoveRestrictedTokens(SplitStringToTokens(review.Title.ToLower()), stopWords);
                string reviewTitleWithoutStopWords = "";

                foreach (var token in reviewTitleWithoutStopWordTokens)
                {
                    reviewTitleWithoutStopWords += token;
                }

                if (MatchReviewTitleWithProductStrings(reviewTitleWithoutStopWords, productTokens))
                {
                    reviewMatches.Add(review); //add review to list of reviews that link to this product
                    review.linkedProducts.Add(this); //add this GPU product to review list of products it links to

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
        }

        public override void MatchReviewAndProduct1(List<Review> reviewList, Dictionary<string, bool> stopWords, ref ReviewProductLinks reviewProductLinks)
        {
            string productStrings = Model.ToLower() + " " + CpuSeries.ToLower();
            List<string> productTokens = RemoveRestrictedTokens(SplitStringToTokens(productStrings), stopWords);


            foreach (var review in reviewList)
            {
                if (review.Category.ToLower() != "cpu")
                    continue;

                if (CompareReviewTitleWithProductStrings1(review.Title.ToLower(), productTokens, stopWords))
                {
                   //add review id to product
                    reviewMatches.Add(review);
                    review.linkedProducts.Add(this);

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
        }

        public override string ToString()
        {
            //return $"{nameof(PhysicalCores)}: {PhysicalCores}, {nameof(LogicalCores)}: {LogicalCores}, {nameof(StockCooler)}: {StockCooler}, {nameof(Model)}: {Model}, {nameof(Clock)}: {Clock}, {nameof(Socket)}: {Socket}, {nameof(MaxTurbo)}: {MaxTurbo}, {nameof(IntegratedGpu)}: {IntegratedGpu}, {nameof(Manufacturer)}: {Manufacturer}, {nameof(CpuSeries)}: {CpuSeries}";
            return $"{nameof(Model)}: {Model}, {nameof(Manufacturer)}: {Manufacturer}, {nameof(CpuSeries)}: {CpuSeries}";
        }

    }

}