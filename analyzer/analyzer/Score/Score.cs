using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using analyzer.Products;
using analyzer.Products.Reviews;

namespace analyzer.Score
{
    class Score
    {
        //todo move/instanciate to proper place
        private static Dictionary<string, double> categoryFactors = new Dictionary<string, double>() { { "GPU", 2 }, { "CPU", 3 } };//category factors: GPU 2, CPU 3


        public void AssessProductScores(Product product)
        {
            DateTime oldestReviewAge = product.reviewMatches[0].ReviewDate;
            //double criticReviewWeightSum = 0;
            //double userReviewWeightSum = 0;
            double criticReviewNumerator = 0,
                criticReviewDenominator = 0,
                userReviewNumerator = 0,
                userReviewDenominator = 0;

            // product category factor
            double productFactor = 1;
            if (categoryFactors.ContainsKey(product.Category))
                productFactor = categoryFactors[product.Category];

            List<double> normalizedCriticScores = new List<double>();
            List<double> normalizedUserScores = new List<double>();


            // find oldest critic review to assess product age
            foreach (Review review in product.reviewMatches)
            {
                if (review.ReviewDate < oldestReviewAge && review.GetType() == typeof(CriticReview))
                    oldestReviewAge = review.ReviewDate;
            }

            // review specific calculations
            foreach (Review review in product.reviewMatches)
            {
                bool isCriticReview = review.GetType() == typeof(CriticReview);

                // review weight (age)
                TimeSpan reviewAge = review.ReviewDate.Subtract(oldestReviewAge);
                double reviewAgeInYears = reviewAge.Days / 365;
                review.reviewWeight = ComputeReviewWeight(reviewAgeInYears, productFactor);

                // review average score
                double normalizedScore = review.Rating / review.MaxRating;

                //compute score
                if (isCriticReview)
                {
                    criticReviewNumerator += normalizedScore * review.reviewWeight;
                    criticReviewDenominator += review.reviewWeight;
                }
                else
                {
                    userReviewNumerator += normalizedScore * review.reviewWeight;
                    userReviewDenominator += review.reviewWeight;
                }
            }

            double criticScore = criticReviewNumerator / criticReviewDenominator;
            double userScore = userReviewNumerator / userReviewDenominator;

            //superscore = critic*weigth + user*weight / sum weight
        }


        public void AssessProductListScores(List<Product> productList)
        {
            foreach (Product product in productList)
            {
                AssessProductScores(product);
            }
        }

        public double ComputeReviewWeight(double age, double categoryFactor)
        {
            //\frac{1}{(1 + 10 * e^{-2 * x + 2.5}) + 1}
            double exponent = -2 * age + 2.5;
            double result = 1 / (1 + 10 * Math.Pow(Math.E, exponent)) + 1;

            return result;
        }

    }
}
