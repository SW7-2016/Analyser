using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using analyzer.Products;
using analyzer.Products.DistinctProductList.types;
using analyzer.Products.Reviews;

namespace analyzer.Score
{
    class Score
    {
        //todo move/instanciate to proper place
        private static readonly Dictionary<string, double> categoryFactors = 
            new Dictionary<string, double>() {
                { "GPU", 2 },
                { "CPU", 4 }
            };//category factors: GPU 2, CPU 4
        private const double UserScoreWeight = 1;
        private const double CriticScoreWeight = 2;


        public static void AssessProductScores(Product product)
        {
            double userScoreWeight = UserScoreWeight;
            double criticScoreWeight = CriticScoreWeight;
            DateTime oldestReviewAge = product.reviewMatches[0].ReviewDate;
            double criticReviewNumerator = 0,
                criticReviewDenominator = 0,
                userReviewNumerator = 0,
                userReviewDenominator = 0;
            bool hasCriticReview = false, hasUserReview = false;

            //debugs
            List<double> criticRatings = new List<double>();
            List<double> userRatings = new List<double>();

            // product category factor
            double productFactor = 1;
            if (categoryFactors.ContainsKey(product.Category))
                productFactor = categoryFactors[product.Category];


            // find oldest critic review to assess product age
            foreach (Review review in product.reviewMatches)
            {
                if (review.ReviewDate < oldestReviewAge && review.GetType() == typeof(CriticReview))
                    oldestReviewAge = review.ReviewDate;
            }

            double productAge = (DateTime.Today.Subtract(oldestReviewAge).Days) / (double) 365;

            // review-specific calculations
            foreach (Review review in product.reviewMatches)
            {
                bool isCriticReview = review.GetType() == typeof(CriticReview);

                // review weight (age)
                TimeSpan reviewAge = review.ReviewDate.Subtract(oldestReviewAge);
                double reviewAgeInYears = reviewAge.Days / (double) 365;
                review.reviewWeight = ComputeReviewWeight(reviewAgeInYears, productFactor);

                // review average score
                double normalizedScore = review.Rating / review.MaxRating;

                //compute score
                if (isCriticReview)
                {
                    criticReviewNumerator += normalizedScore * review.reviewWeight;
                    criticReviewDenominator += review.reviewWeight;
                    hasCriticReview = true;
                    criticRatings.Add(review.Rating);//debug
                }
                else
                {
                    userReviewNumerator += normalizedScore * review.reviewWeight;
                    userReviewDenominator += review.reviewWeight;
                    hasUserReview = true;
                    userRatings.Add(review.Rating);//debug
                }
            }


            double criticScore = 0, 
                userScore = 0;

            if (hasCriticReview)
                criticScore = criticReviewNumerator/criticReviewDenominator;

            if(hasUserReview)
                userScore = userReviewNumerator / userReviewDenominator;

            if (!hasCriticReview)
                criticScoreWeight = 0;

            if (!hasUserReview)
                userScoreWeight = 0;

            //superscore = critic*weigth + user*weight / sum weight
            double averageScore = ((criticScore*criticScoreWeight) + (userScore*userScoreWeight))/
                                (criticScoreWeight + userScoreWeight);

            double tempSuperScore = averageScore * ComputeDecayWeight(productAge, productFactor) + 0.01;
            int superScore = (int) Math.Round(tempSuperScore*100);
            
            // set scores on product
            setProductScores(product,superScore,criticScore,userScore);
            
        }

        private static void setProductScores(Product product, int superScore, double criticScore, double userScore)
        {
            product.criticScore = criticScore;
            product.userScore = userScore;
            product.superScore = superScore;
        }


        public static void AssessProductListScores<T> (List<T> productList) where T : Product
        {
            int reviews = 0, products = 0;
            Dictionary<int, int> superscores = new Dictionary<int, int>();
            foreach (var product in productList)
            {
                products += 1;
                if (product.reviewMatches.Count > 0)
                {
                    AssessProductScores(product);
                    superscores.Add(product.Id, product.superScore);
                    reviews += 1;
                }
            }
            int i = 1;
        }

        public static double ComputeReviewWeight(double age, double categoryFactor)
        {
            //\frac{1}{(1 + 10 * e^{-2 * x + 2.5}) + 1}
            double exponent = -2 * age + 2.5;
            double result = 1 / (1 + 10 * Math.Pow(Math.E, exponent)) + 1;

            return result;
        }

        public static double ComputeDecayWeight(double age, double halfPoint)
        {
            // 1/(1 + e^(c * x/b - c))
            double exponent = 5 * age/halfPoint - 5;
            double result = 1 / (1 + Math.Pow(Math.E, exponent));

            return result;
        }

        

    }
}
