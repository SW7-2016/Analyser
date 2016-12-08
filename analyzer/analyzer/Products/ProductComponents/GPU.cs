using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Automation.Peers;
using analyzer.CompareAndMerge;
using analyzer.Products.Reviews;

namespace analyzer.Products.ProductComponents
{
    public class GPU : ComputerComponents
    {
        public GPU(string category, int id, string name, string processorManufacturer, string chipset, string graphicsProcessor, 
                    string architecture, string cooling, string memSize, int pciSlots, string manufacturer, string model) 
            : base(id, category, name)
        {
            ProcessorManufacturer = processorManufacturer;
            Chipset = chipset;
            GraphicsProcessor = graphicsProcessor;
            Architecture = architecture;
            Cooling = cooling;
            MemSize = memSize;
            Manufacturer = manufacturer;
            PciSlots = pciSlots;
            Model = model;
        }

        public int PciSlots { get; }
        public string ProcessorManufacturer { get; }
        public string Chipset { get; }
        public string GraphicsProcessor { get; }
        public string Model { get; }
        public string Architecture { get; }
        public string Cooling { get; }
        public string MemSize { get; }
        public string Manufacturer { get; }

        public override void MatchReviewAndProduct(List<Review> reviewList, Dictionary<string, bool> stopWords, ref ReviewProductLinks reviewProductLinks)
        {//linking method which uses the title.contains(productToken) way for linking
            List<string> productTokens = SplitStringToTokens(Model.ToLower() + " " + GraphicsProcessor.ToLower() + " " + Manufacturer.ToLower());
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
            string productStrings = Model.ToLower() + " " + GraphicsProcessor.ToLower() + " " + Manufacturer.ToLower();
            List<string> productTokens = RemoveRestrictedTokens(SplitStringToTokens(productStrings), stopWords);

            foreach (var review in reviewList)
            {
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
            return
                $"{nameof(ProcessorManufacturer)}: {ProcessorManufacturer}, {nameof(GraphicsProcessor)}: {GraphicsProcessor}, {nameof(Model)}: {Model}, {nameof(Manufacturer)}: {Manufacturer}";
        }
    }
}
