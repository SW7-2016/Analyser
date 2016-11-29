using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Automation.Peers;
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

            foreach (var review in reviewList)
            {
                manufactureMatch = false;
                if (review.Category != "GPU")
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
                //Debug.WriteLine(this.ToString());
                reviewMatches.Add(review.Id);
            }
        }

        public override void MatchReviewAndProduct2<T>(List<Review> reviewList, List<T> productList)
        { 
            bool manufactureMatch = false; //Must match
            //bool graphicsProcessorMatch = false; //Number must match
            //bool processorManufacturerMatch = false; //Must match
            //bool modelMatch = false; //Must match
            int count = 0;

            foreach (var review in reviewList)
            {
                manufactureMatch = false;
                if (review.Category != "GPU")
                    continue;

                foreach (string token in review.TokenList)
                {
                    if (token == Manufacturer.ToLower())
                    {
                        manufactureMatch = true;
                        break;
                    }
                }

                /*if (!manufactureMatch)
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

                if (!CompareModelStrings2(review.Title.ToLower(), Model.ToLower(), GraphicsProcessor.ToLower()))
                {
                    continue;
                }*/

                if (!manufactureMatch || !CompareModelStrings3(review.Title.ToLower(), Model.ToLower() + " " + GraphicsProcessor.ToLower() + " " + Manufacturer.ToLower()))
                {
                    continue;
                }
                count++;
                Debug.WriteLine(this.ToString());
                Debug.WriteLine(review.Title);
                Debug.WriteLine(count);
                reviewMatcheConcatenate.Add(new KeyValuePair<int, string>(review.Id, review.Title));
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

            reviewTitleStrings.Remove("gtx");
            reviewTitleStrings.Remove("geforce");

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
        private bool CompareModelStrings2(string reviewTitle, string model, string graphicsProcessor)
        {
            List<string> modelTokens = SplitStringToTokens(model);
            List<string> graphicsProcessorTokens = SplitStringToTokens(graphicsProcessor);
            string concatenatedReviewTitle = ConcatenateString(reviewTitle);
            int count = 0;

            concatenatedReviewTitle = concatenatedReviewTitle.ToLower().Replace("gtx", "");
            concatenatedReviewTitle = concatenatedReviewTitle.ToLower().Replace("geforce", "");
            concatenatedReviewTitle = concatenatedReviewTitle.ToLower().Replace("radeon", "");

            foreach (var token in graphicsProcessorTokens)
            {
                modelTokens.Remove(token);
            }

            foreach (var token in modelTokens)
            {

                if (concatenatedReviewTitle.Contains(token))
                {
                    concatenatedReviewTitle = concatenatedReviewTitle.Replace(token, "");
                    count++;
                }
            }
            if (count == modelTokens.Count)
            {
                return true;
            }


            return false;
        }

        private bool CompareModelStrings3(string reviewTitle, string allMighty)
        {
            List<string> allTokens = SplitStringToTokens(allMighty);
            List<string> newAllTokens = new List<string>();

            foreach (var token in allTokens)
            {
                if (!newAllTokens.Contains(token) && token != "nvidia" && token != "radeon" && token != "geforce"
                     && token != "amd" && token != "gtx")
                {
                    newAllTokens.Add(token);
                }
            }

            newAllTokens = newAllTokens.OrderByDescending(item => item.Length).ToList();

            string concatenatedReviewTitle = ConcatenateString(reviewTitle);
            int count = 0;

            concatenatedReviewTitle = concatenatedReviewTitle.ToLower().Replace("gtx", "");
            concatenatedReviewTitle = concatenatedReviewTitle.ToLower().Replace("geforce", "");
            concatenatedReviewTitle = concatenatedReviewTitle.ToLower().Replace("radeon", "");
            concatenatedReviewTitle = concatenatedReviewTitle.ToLower().Replace("amd", "");
            concatenatedReviewTitle = concatenatedReviewTitle.ToLower().Replace("nvidia", "");


            foreach (var token in newAllTokens)
            {

                if (concatenatedReviewTitle.Contains(token))
                {
                    concatenatedReviewTitle = concatenatedReviewTitle.Replace(token, "");
                    count++;
                }
            }
            if (count == newAllTokens.Count)
            {
                return true;
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
