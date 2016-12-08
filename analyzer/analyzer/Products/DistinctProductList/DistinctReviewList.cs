using analyzer.Products.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace analyzer.Products.DistinctProductList
{
    public class DistinctReviewList<T> : List<T> where T : Review
    {
        public Dictionary<int, List<Review>> prunedList = new Dictionary<int, List<Review>>();

        public DistinctReviewList() { }

        public new void Add(T item)
        {
            base.Add(item);

            MatchCollection numbers = Regex.Matches(item.Title, "\\d+\\.\\d+|\\d+");

            List<int> nonDupNumbers = new List<int>();

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

            if (nonDupNumbers.Count() == 0)
            {
                if (prunedList.ContainsKey(0))
                {
                    prunedList[0].Add(item);
                }
                else
                {
                    prunedList.Add(0, new List<Review>() { item });
                }
            }

            foreach (int number in nonDupNumbers)
            {

                if (((item.Category.ToLower() == "gpu" && number > 0) || (item.Category.ToLower() == "cpu" && number > 20))
                    && number < 15000)
                {
                    if (prunedList.ContainsKey(number))
                    {
                        prunedList[number].Add(item);
                    }
                    else
                    {
                        prunedList.Add(number, new List<Review>() { item });
                    }
                }
            }
        }

        public void AddDistinctList(DistinctReviewList<Review> old)
        {
            foreach (KeyValuePair<int, List<Review>> item in old.prunedList)
            {
                if (prunedList.ContainsKey(item.Key))
                {
                    foreach (var listField in item.Value)
                    {
                        prunedList[item.Key].Add(listField);
                    }
                }
                else
                {
                    prunedList.Add(item.Key, item.Value);
                } 
            }

            base.AddRange((IEnumerable<T>)old.ToArray());
        }

        public DistinctReviewList<Review> ToReview<T>() where T : Review
        {
            DistinctReviewList<Review> result = new DistinctReviewList<Review>();

            foreach (var item in this)
            {
                result.Add(item);
            }

            return result;
        }
    }
}
