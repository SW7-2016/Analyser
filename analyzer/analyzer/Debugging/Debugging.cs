using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using analyzer.CompareAndMerge;
using analyzer.Products.ProductComponents;
using analyzer.Products;
using analyzer.Products.Reviews;

namespace analyzer.Debugging
{
    class Debugging
    {
        #region Catch Reviews with more than one link 
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
        #endregion

        #region Show all reviews whitout a link
        public static void GetUnlinkedReviews(List<Review> allReviews, List<Chassis> chassisList, List<CPU> cpuList, List<GPU> gpuList,
            List<HardDrive> hardDriveList, List<Motherboard> motherboardList, List<PSU> psuList, List<RAM> ramList)
        {
            List<int> reviewIdList = new List<int>();
            List<int> allReviewsIds = new List<int>();

            //Add all review ids ti list
            foreach (var review in allReviews)
            {
                allReviewsIds.Add(review.Id);
            }

           

            #region add to shared list
            //add reviews with links to reviewIdList
            foreach (var chassis in chassisList)
            {
                reviewIdList = helper2(reviewIdList, chassis);

            }
            foreach (var cpu in cpuList)
            {
                reviewIdList = helper2(reviewIdList, cpu);

            }
            foreach (var gpu in gpuList)
            {
                reviewIdList = helper2(reviewIdList, gpu);

            }
            foreach (var hardDrive in hardDriveList)
            {
                reviewIdList = helper2(reviewIdList, hardDrive);

            }
            foreach (var motherboard in motherboardList)
            {
                reviewIdList = helper2(reviewIdList, motherboard);
            }
            foreach (var psu in psuList)
            {
                reviewIdList = helper2(reviewIdList, psu);

            }
            foreach (var ram in ramList)
            {
                reviewIdList = helper2(reviewIdList, ram);

            }
            #endregion

            reviewIdList = allReviewsIds.Except(reviewIdList).ToList();
            reviewIdList.Sort();

            bool first = true;
            StringBuilder sb = new StringBuilder();
            foreach (int id in reviewIdList)
            {
                if (first)
                {
                    sb.Append("select * from Review where ReviewID = ");
                    sb.Append(id);
                    first = false;
                }
                else
                {
                    sb.Append(" or ReviewID = ");
                    sb.Append(id);
                }
            }
            sb.Append(";");
            Debug.WriteLine(sb);
        }

        private static List<int> helper2<T>(List<int> reviewIdList, T product) where T : Product
        {
            foreach (var reviewID in product.reviewMatches)
            {
                if (!reviewIdList.Contains(reviewID))
                {
                    reviewIdList.Add(reviewID);
                }
            }

            return reviewIdList;
        }
        #endregion

        #region Show number of review for each product of a single type.
        public static void NumberOfReviewForEachProduct<T>(List<T> productList) where T : Product
        {
            int largest = 0;
            int id = 0;
            int total = 0;
            int withReviews = 0;

            foreach (var product in productList)
            {
                if (product.reviewMatches.Count > largest)
                {
                    largest = product.reviewMatches.Count;
                    id = product.Id;
                }

                if (product.reviewMatches.Count > 0)
                {
                    Debug.WriteLine("{0} reviews on Product with ID: {1} ", product.reviewMatches.Count, product.Id);
                    withReviews++;
                }
                total++;
            }
            Debug.WriteLine("");
            Debug.WriteLine("Largest set of reviews is {0} on product id {1}", largest, id);
            Debug.WriteLine("{0} out of {1} have minimum one review linked", withReviews, total);
        }
        #endregion
    }
}
