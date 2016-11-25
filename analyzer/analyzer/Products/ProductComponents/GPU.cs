using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using analyzer.Products.Reviews;

namespace analyzer.Products.ProductComponents
{
    public class GPU : ComputerComponents
    {
        public GPU(string category, int id, string name, string processorManufacturer, string chipset, string graphicsProcessor, 
                    string architecture, string cooling, string memSize, int pciSlots, string manufacturer) 
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
        }

        public int PciSlots { get; }
        public string ProcessorManufacturer { get; }
        public string Chipset { get; }
        public string GraphicsProcessor { get; }
        public string Architecture { get; }
        public string Cooling { get; }
        public string MemSize { get; }
        public string Manufacturer { get; }

       

        public override void MatchReviewAndProduct<T>(List<Review> reviewList, List<T> productList)
        { 
            bool manufactureMatch = false; //Must match
            bool graphicsProcessorMatch = false; //Number must match
            bool processorManufacturerMatch = false; //Must match
            bool modelMatch = false; //Must match

            List<int> matchingReviewsList = new List<int>();

            foreach (var review in reviewList)
            {
                foreach (string token in review.TokenList)
                {
                    if (token == Manufacturer.ToLower())
                    {
                        manufactureMatch = true;
                    }
                }

                if (CompareGraphicsProcessor(review.Title, GraphicsProcessor))
                {
                    graphicsProcessorMatch = true;
                }

                if (CompareGraphicsProcessorStrings(review.Title.ToLower(), GraphicsProcessor.ToLower()))
                {
                    processorManufacturerMatch = true;
                }


                if (manufactureMatch && graphicsProcessorMatch && processorManufacturerMatch && modelMatch)
                {
                    reviewMatches.Add(review.Id);
                }
            }
        }

        private bool CompareGraphicsProcessor(string reviewTitle, string graphicsProcessor)
        {
            MatchCollection GPUprocessorNumbers =  ExtractNumbersFromString(graphicsProcessor);
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

        private bool CompareGraphicsProcessorStrings(string reviewTitle, string graphicsProcessor)
        {
            List<string> graphicsProcessorStrings = SplitStringToTokens(graphicsProcessor);
            List<string> reviewTitleStrings = SplitStringToTokens(reviewTitle);
            int GPUCount = 0;
            int reviewCount = 0;

            foreach (string gpuString in graphicsProcessorStrings)
            {
                foreach (string reviewString in reviewTitleStrings)
                {
                    if (gpuString == reviewString)
                    {
                        return true;
                    }
                    reviewCount++;
                }
                GPUCount++;
            }

            return false;

        }
    }
}
