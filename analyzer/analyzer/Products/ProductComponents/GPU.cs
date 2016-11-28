using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
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

       

        public override void MatchReviewAndProduct<T>(List<Review> reviewList, List<T> productList)
        { 
            bool manufactureMatch = false; //Must match
            //bool graphicsProcessorMatch = false; //Number must match
            //bool processorManufacturerMatch = false; //Must match
            //bool modelMatch = false; //Must match

            List<int> matchingReviewsList = new List<int>();


            foreach (var review in reviewList)
            {
                manufactureMatch = false;

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

                if (!CompareGraphicsProcessor(review.Title, GraphicsProcessor))
                {
                    continue;
                }

                if (!CompareGraphicsProcessorStrings(review.Title.ToLower(), GraphicsProcessor.ToLower()))
                {
                    continue;
                }

                if (!CompareModelStrings(review.Title.ToLower(), Model.ToLower(), GraphicsProcessor.ToLower()))
                {
                    continue;
                }
                Debug.WriteLine(this.ToString());
                reviewMatches.Add(review.Id);
            }
        }

        public override string ToString()
        {
            return $"{nameof(ProcessorManufacturer)}: {ProcessorManufacturer}, {nameof(GraphicsProcessor)}: {GraphicsProcessor}, {nameof(Model)}: {Model}, {nameof(Manufacturer)}: {Manufacturer}";
        }

        private bool CompareGraphicsProcessor(string reviewTitle, string graphicsProcessor)
        {
            MatchCollection gpuProcessorNumbers = ExtractNumbersFromString(graphicsProcessor);
            MatchCollection reviewTitleNumbers = ExtractNumbersFromString(reviewTitle);

            foreach (Match gpuNumber in gpuProcessorNumbers)
            {
                foreach (Match reviewNumber in reviewTitleNumbers)
                {
                    if (int.Parse(gpuNumber.Value) == int.Parse(reviewNumber.Value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CompareModelStrings(string reviewTitle, string model, string graphicsProcessor)
        {
            List<string> graphicsProcessorStrings = SplitStringToTokens(graphicsProcessor);
            List<string> modelStrings = SplitStringToTokens(model);
            List<string> actualModelStrings = SplitStringToTokens(model);
            List<string> reviewTitleStrings = SplitStringToTokens(reviewTitle);
            int count = 0;

            foreach (string modelString in modelStrings)
            {
                foreach (string graphicsProcessorString in graphicsProcessorStrings)
                {
                    if (graphicsProcessorString == modelString)
                    {
                        actualModelStrings.Remove(graphicsProcessorString);
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
                    if (count == actualModelStrings.Count - 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool CompareModelStrings(string reviewTitle, string model, string graphicsProcessor)
        {
            List<string> graphicsProcessorStrings = SplitStringToTokens(graphicsProcessor);
            List<string> modelStrings = SplitStringToTokens(model);
            List<string> actualModelStrings = SplitStringToTokens(model);
            List<string> reviewTitleStrings = SplitStringToTokens(reviewTitle);
            int count = 0;

            foreach (string modelString in modelStrings)
            {
                foreach (string graphicsProcessorString in graphicsProcessorStrings)
                {
                    if (graphicsProcessorString == modelString)
                    {
                        actualModelStrings.Remove(graphicsProcessorString);
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
                    if (count == actualModelStrings.Count - 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CompareGraphicsProcessorStrings(string reviewTitle, string graphicsProcessor)
        {
            List<string> graphicsProcessorStrings = SplitStringToTokens(graphicsProcessor);
            List<string> reviewTitleStrings = SplitStringToTokens(reviewTitle);

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
    }
}
