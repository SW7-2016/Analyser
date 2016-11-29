using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using analyzer.CompareAndMerge;
using analyzer.Products.ProductComponents;
using analyzer.Products;

namespace analyzer.Debugging
{
    class Debugging
    {
        public static void DebugReviewDuplicates(List<Chassis> chassisList, List<CPU> cpuList, List<GPU> gpuList, List<HardDrive> hardDriveList, List<Motherboard> motherboardList, List<PSU> psuList, List<RAM> ramList)
        {
            List<ProductReviewId> reviewIdList = new List<ProductReviewId>();

            #region add to shared list
            //add review and product ids to list
            foreach (var chassis in chassisList)
            {
                reviewIdList = helper(reviewIdList, chassis);

            }
            foreach (var cpu in cpuList)
            {
                reviewIdList = helper(reviewIdList, cpu);

            }
            foreach (var gpu in gpuList)
            {
                reviewIdList = helper(reviewIdList, gpu);

            }
            foreach (var hardDrive in hardDriveList)
            {
                reviewIdList = helper(reviewIdList, hardDrive);

            }
            foreach (var motherboard in motherboardList)
            {
                reviewIdList = helper(reviewIdList, motherboard);
            }
            foreach (var psu in psuList)
            {
                reviewIdList = helper(reviewIdList, psu);

            }
            foreach (var ram in ramList)
            {
                reviewIdList = helper(reviewIdList, ram);

            }
#endregion

            //display reviews which occoure more than once
            foreach (ProductReviewId link in reviewIdList)
            {
                int tempCount = 0;
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * FROM CPU WHERE");
                

                if (link.ProductId.Count > 1)
                {
                    Debug.WriteLine("Review ID {0} is reffered to by:", link.ReviewId);
                    foreach (int product in link.ProductId)
                    {
                        Debug.WriteLine("ProductID: {0}", product);
                        if (tempCount < 1)
                        {
                            sb.Append(" ProductID = ");
                            sb.Append(product);
                        }
                        else
                        {
                            sb.Append(" OR ProductID = ");
                            sb.Append(product);
                        }
                        tempCount++;
                    }

                    sb.Append(";");
                    Debug.WriteLine("As SQL:");
                    Debug.WriteLine(sb);
                    Debug.WriteLine("");
                }
            }
        }

        private static List<ProductReviewId> helper<T>(List<ProductReviewId> reviewIdList, T product) where T : Product
        {
            bool temp = false;

            foreach (var instans in product.reviewMatches) // instace = Review id
            {
                foreach (var element in reviewIdList)
                {
                    if (element.ReviewId == instans)
                    {
                        element.ProductId.Add(product.Id);
                        temp = true;
                    }
                }
                if (!temp)
                {
                    List<int> TList = new List<int>();
                    TList.Add(product.Id);

                    reviewIdList.Add(new ProductReviewId(TList, instans));
                }
            }
            return reviewIdList;
        }
    }
}
