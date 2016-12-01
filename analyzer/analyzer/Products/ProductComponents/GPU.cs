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

        public override void MatchReviewAndProduct(List<Review> reviewList, ReviewProductLinks reviewProductLinks)
        {
            List<string> restrictedTokens = new List<string>();

            restrictedTokens.Add("gtx");
            restrictedTokens.Add("geforce");
            restrictedTokens.Add("nvidia");
            restrictedTokens.Add("amd");
            restrictedTokens.Add("radeon");

            foreach (var review in reviewList)
            {
                bool manufactureMatch = false;

                if (review.Category.ToLower() != "gpu")
                    continue;

                foreach (string token in review.TokenList)
                {
                    if (token == Manufacturer.ToLower())
                    {
                        manufactureMatch = true;
                        break;
                    }
                }

                if (!manufactureMatch)
                {
                    continue;
                }

                if (!MatchStringNumbers(GraphicsProcessor, review.Title))
                {
                    continue;
                }

                if (!CompareGraphicsProcessorStrings(review.Title.ToLower(), GraphicsProcessor.ToLower(), restrictedTokens))
                {
                    continue;
                }

                if (!CompareModelStrings(review.Title.ToLower(), Model.ToLower(), GraphicsProcessor.ToLower(), restrictedTokens))
                {
                    continue;
                }
                //check if added review is correct in debug console
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

        /*public override void MatchReviewAndProduct<T>(List<Review> reviewList, List<T> productList)
        {
            List<string> restrictedTokens = new List<string>();
            string productStrings = Model.ToLower() + " " + GraphicsProcessor.ToLower() + " " + Manufacturer.ToLower();
            

            restrictedTokens.Add("gtx");
            restrictedTokens.Add("geforce");
            restrictedTokens.Add("nvidia");
            restrictedTokens.Add("amd");
            restrictedTokens.Add("radeon");

            productStrings = RemoveRestrictedTokens(productStrings, restrictedTokens);
           

            foreach (var review in reviewList)
            {
                if (review.Category.ToLower() != "gpu")
                    continue;

                string concatenatedReviewTitle = RemoveRestrictedTokens(ConcatenateString(review.Title.ToLower()), restrictedTokens);

                if (!MatchStringToTokens(Manufacturer.ToLower(), review.TokenList) || !CompareReviewTitleWithProductStrings(concatenatedReviewTitle, productStrings))
                {
                    continue;
                }

                //check if added review is correct in debug console
                Debug.WriteLine(this.Id + " " + this.ToString());
                Debug.WriteLine(review.Title);
                //add review id to product
                reviewMatches.Add(review.Id);
            }
        }*/

        public override string ToString()
        {
            return $"{nameof(ProcessorManufacturer)}: {ProcessorManufacturer}, {nameof(GraphicsProcessor)}: {GraphicsProcessor}, {nameof(Model)}: {Model}, {nameof(Manufacturer)}: {Manufacturer}";
        }

        private bool CompareGraphicsProcessorStrings(string reviewTitle, string graphicsProcessor, List<string> restrictedTokens) 
        {
            List<string> graphicsProcessorStrings = SplitStringToTokens(RemoveRestrictedTokens(graphicsProcessor, restrictedTokens));
            List<string> reviewTitleStrings = SplitStringToTokens(RemoveRestrictedTokens(reviewTitle, restrictedTokens));

            foreach (string gpuString in graphicsProcessorStrings)
            {
                foreach (string reviewString in reviewTitleStrings)
                {
                    if (gpuString == reviewString)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CompareModelStrings(string reviewTitle, string model, string graphicsProcessor, List<string> restrictedTokens)
        {
            List<string> graphicsProcessorStrings = SplitStringToTokens(RemoveRestrictedTokens(graphicsProcessor, restrictedTokens));
            List<string> modelStrings = SplitStringToTokens(RemoveRestrictedTokens(model, restrictedTokens));
            List<string> reviewTitleStrings = SplitStringToTokens(RemoveRestrictedTokens(reviewTitle, restrictedTokens)).Distinct().ToList();
            List<string> actualModelStrings = new List<string>();
            int count = 0;

            foreach (string modelString in modelStrings)
            {
                foreach (string graphicsProcessorString in graphicsProcessorStrings)
                {
                    if (modelString != graphicsProcessorString  && !actualModelStrings.Contains(modelString))
                    {
                        actualModelStrings.Add(modelString);
                    }
                }
            }

            foreach (string modelString in actualModelStrings)
            {
                foreach (string reviewString in reviewTitleStrings)
                {
                    if (modelString == reviewString)
                    {
                        count++;
                    }
                    if (count == actualModelStrings.Count)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
