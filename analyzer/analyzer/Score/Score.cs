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
        private const double CriticReviewQuantityThreshold = 1;
        private const double UserReviewQuantityThreshold = 10;


        public static void AssessProductScores(Product product)
        {
            double userScoreWeight = UserScoreWeight;
            double criticScoreWeight = CriticScoreWeight;
            DateTime oldestReviewDate = product.reviewMatches[0].ReviewDate;
            DateTime newestReviewDate = oldestReviewDate;
            double criticReviewNumerator = 0,
                criticReviewDenominator = 0,
                userReviewNumerator = 0,
                userReviewDenominator = 0,
                criticScore = 0,
                userScore = 0,
                totalReviews = 0;

            int maxDiffVotes = 0, minDiffVotes = 999999;
            bool hasCriticReview = false, hasUserReview = false;

            //debugs
            List<double> criticRatings = new List<double>();
            List<double> userRatings = new List<double>();

            // product category factor
            double productFactor = 4;// default decay weight
            if (categoryFactors.ContainsKey(product.Category))
                productFactor = categoryFactors[product.Category];

            // find oldest review to assess product age
            foreach (Review review in product.reviewMatches)
            {
                if (review.Rating == -1)
                    continue;
                if (review.ReviewDate < oldestReviewDate)
                    oldestReviewDate = review.ReviewDate;
                if (review.ReviewDate > newestReviewDate)
                    newestReviewDate = review.ReviewDate;

                if (review.GetType() == typeof(CriticReview))
                {
                    criticRatings.Add(review.Rating/review.MaxRating);
                    review.isCritic = true;
                }
                else
                {
                    int diff = review.positiveReception - review.negativeReception;
                    if (diff > maxDiffVotes)
                        maxDiffVotes = diff;
                    else if (diff < minDiffVotes)
                        minDiffVotes = diff;
                    userRatings.Add(review.Rating / review.MaxRating);
                }
            }

            // If not sufficient amount of reviews, do not assess superscore
            if (criticRatings.Count < CriticReviewQuantityThreshold &&
                userRatings.Count < UserReviewQuantityThreshold)
                return;

            // Assume age 
            double productAge = (DateTime.Today.Subtract(oldestReviewDate).Days) / (double) 365;

            // review-specific calculations
            foreach (Review review in product.reviewMatches)
            {
                if (review.Rating == -1)
                    continue;

                // review weight (age)
                TimeSpan reviewAge = review.ReviewDate.Subtract(oldestReviewDate);
                double reviewAgeInYears = reviewAge.Days / (double) 365;
                review.reviewWeight = ComputeReviewWeight(reviewAgeInYears, productFactor);

                // review average score
                
                double weightedReviewScore = review.normalizedScore*review.reviewWeight;
                //compute score
                if (review.isCritic)
                {
                    review.normalizedScore = review.Rating / review.MaxRating;
                    criticReviewNumerator += review.normalizedScore * review.reviewWeight;
                    criticReviewDenominator += review.reviewWeight;
                    hasCriticReview = true;
                }
                else
                {
                    double normalizedScore = review.Rating / review.MaxRating;

                    review.normalizedScore = normalizedScore;
                    totalReviews += HelpfulFactor(maxDiffVotes, minDiffVotes, review.positiveReception, review.negativeReception, review);

                    userReviewNumerator += review.normalizedScore * review.reviewWeight;
                    userReviewDenominator += review.reviewWeight;
                    hasUserReview = true;
                }
            }

            if (hasCriticReview)
                criticScore = criticReviewNumerator / criticReviewDenominator;

            if(hasUserReview)
                userScore = userReviewNumerator / userReviewDenominator;

            if (!hasCriticReview)
                criticScoreWeight = 0;

            if (!hasUserReview)
                userScoreWeight = 0;

            //superscore = critic*weigth + user*weight / sum weight
            double weightedAverageScore = ((criticScore*criticScoreWeight) + (userScore*userScoreWeight))/
                                (criticScoreWeight + userScoreWeight);

            double tempSuperScore = weightedAverageScore * ComputeDecayWeight(productAge, productFactor) + 0.01;

            if (hasUserReview)
                userScore = AverageWeightedUserScore(product.reviewMatches, totalReviews);
            if(hasCriticReview)
                criticScore = criticRatings.Average();

            int superScore = (int) Math.Round(tempSuperScore*100);
            int avgUserScore = (int)Math.Round(userScore * 100);
            int avgCriticScore = (int)Math.Round(criticScore * 100);

            // set scores on product
            SetProductScores(product,superScore,avgCriticScore,avgUserScore);
            product.newestReviewDate = newestReviewDate;
            product.oldestReviewDate = oldestReviewDate;
        }

        private static double AverageWeightedUserScore(List<Review> reviews, double totalReviews)
        {
            double totalScore = 0;
            foreach (var review in reviews)
            {
                if (review.isCritic == false)
                {
                    totalScore += (review.normalizedScore * review.reviewReceptionModifier);
                }
            }
            return (totalScore/totalReviews);

        }
            
        private static double HelpfulFactor(double maxDiffVotes, double minDiffVotes, double upvotes, double downvotes, Review review)
        {
            double result = ((upvotes - downvotes) - minDiffVotes)/(maxDiffVotes - minDiffVotes);
            review.reviewReceptionModifier = result;
            return result;
        }

        private static void SetProductScores(Product product, int superScore, int criticScore, int userScore)
        {
            product.criticScore = criticScore;
            product.userScore = userScore;
            product.superScore = superScore;
            product.scoreAssessed = true;
        }


        public static List<T> AssessProductListScores<T> (List<T> productList) where T : Product
        {
            List<T> scoredProducts = new List<T>();

            foreach (var product in productList)
            {
                AssessProductScores(product);
                if (product.scoreAssessed)// happens when review has no rating
                    scoredProducts.Add(product);
            }

            return scoredProducts;
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
