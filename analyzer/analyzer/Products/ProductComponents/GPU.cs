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
                int nrOfReviewLinksToProduct = 0;

                if (!MatchStringToTokens(Manufacturer.ToLower(), review.TokenList) || !CompareReviewTitleWithProductStrings(concatenatedReviewTitle, productStrings))
                {
                    continue;
                }
                //check if added review is correct in debug console
                nrOfReviewLinksToProduct++;
                Debug.WriteLine(this.ToString());
                Debug.WriteLine(review.Title);
                Debug.WriteLine(nrOfReviewLinksToProduct);
                //add review id to product
                reviewMatches.Add(review.Id);
            }
        }

        public override string ToString()
        {
            return $"{nameof(ProcessorManufacturer)}: {ProcessorManufacturer}, {nameof(GraphicsProcessor)}: {GraphicsProcessor}, {nameof(Model)}: {Model}, {nameof(Manufacturer)}: {Manufacturer}";
        }
    }
}
