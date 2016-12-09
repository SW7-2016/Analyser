using analyzer.DistinctProductList;
using analyzer.Products.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace analyzer.Products.DistinctProductList
{
    public class DistinctReviewList<T> : DistinctList<T> where T : Review
    {
        public List<int[]> testPruning = new List<int[]>();
        public Dictionary<int, List<Review>> prunGroups = new Dictionary<int, List<Review>>();

        public DistinctReviewList(Dictionary<int, List<Review>> PrunGroups, List<int[]> TestPruning, List<T> old)
        {
            prunGroups = PrunGroups;
            testPruning = TestPruning;
            this.AddRange(old);
        }

        public DistinctReviewList() { }
        
        public new void Add(T item)
        {
            base.Add(item);

            //Also add the review to the prunGroup
            CreatePruningList(item);
        }

        private void CreatePruningList(T review)
        {
            //Find all numbers in the review title.
            MatchCollection numbers = Regex.Matches(review.Title, "\\d+\\.\\d+|\\d+");

            List<int> nonDupNumbers = new List<int>();

            //remove duplicates and non-intigers.
            foreach (Match number in numbers)
            {
                if (!number.Value.Contains(".") && number.Value.Count() < 6)
                {
                    int value = int.Parse(number.Value);

                    if (!nonDupNumbers.Contains(value))
                    {
                        nonDupNumbers.Add(value);
                    }
                }
            }

            //If no prunable token is found, add to list 0, so that is is compared to all products that cant be placed in a prungroup
            if (nonDupNumbers.Count() == 0)
            {
                AddToPrun(0, review);
            }

            //Adds a review to a prunGroup. So it is only compared to the products in same prungroup
            foreach (int number in nonDupNumbers)
            {

                if ((review.Category.ToLower() == "gpu" && isGpuNumberPrunable(number))
                    || (review.Category.ToLower() == "cpu" && isCpuNumberPrunable(number)))
                {
                    AddToPrun(number, review);
                }
            }
        }

        //If prunGroup exists, add the review to that prun group. Else, add new prunGroup, and add review to that.
        private void AddToPrun(int number, Review review)
        {
            if (prunGroups.ContainsKey(number))
            {
                prunGroups[number].Add(review);
                foreach (int[] testRow in testPruning)
                {
                    if (testRow[0] == number)
                    {
                        testRow[1]++;
                        break;
                    }
                }
            }
            else
            {
                prunGroups.Add(number, new List<Review>() { review });
                testPruning.Add(new int[2] { number, 1 });
            }
        }

        //Same as AddRange, but returns DistinctReviewList<Review> and not DistinctReviewList<T>.
        public void AddDistinctList(DistinctReviewList<Review> addition)
        {
            foreach (KeyValuePair<int, List<Review>> prunGroup in addition.prunGroups)
            {
                foreach (Review review in prunGroup.Value)
                {
                    AddToPrun(prunGroup.Key, review);
                }
            }

            AddRange((IEnumerable<T>)addition.ToArray());
        }

        //Changes a DistinctReviewList<Review> to a DistinctReviewList<T>
        public DistinctReviewList<Review> ToReview()
        { 
            return new DistinctReviewList<Review> (this.prunGroups, this.testPruning, this.Cast<Review>().ToList());
        }

    }
}
