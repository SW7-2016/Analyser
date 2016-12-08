using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using analyzer.Products;
using analyzer.Products.ProductComponents;
using analyzer.Products.Reviews;

namespace analyzer.Score
{
    class Score2
    {
        private readonly Dictionary<string, double> categoryFactors =
            new Dictionary<string, double>() {
                { "GPU", 2 },
                { "CPU", 4 }
            };//category factors: GPU 2, CPU 4
        private const double UserScoreWeight = 1;
        private const double CriticScoreWeight = 2;
        private const double CriticReviewQuantityThreshold = 1;
        private const double UserReviewQuantityThreshold = 10;

        public void CalculateProductScores(List<Product> productList)
        {
            foreach (var product in productList)
            {
                CalculateProductScore(product);
            }
        }

        private void CalculateProductScore(Product product)
        {
            int criticReviewAmount = 0;
            int userReviewAmount = 0;
            int userScore;
            int criticScore;
            int superScore;

            foreach (var review in product.reviewMatches)
            {
                review.normalizedScore = NormalizeRating(review.Rating, review.MaxRating);

                if (review.isCritic && review.Rating != -1)
                {
                    criticReviewAmount++;
                }
                else if (!review.isCritic && review.Rating != -1)
                {
                    userReviewAmount++;
                }
            }

            CalculateReviewWeight(product); //Burde critic og userscore at være påvirket af tid?
            if (userReviewAmount > 0)
            {
                userScore = (int)CalculateUserScore(product);
            }
            else
            {
                userScore = -1;
            }

            if (criticReviewAmount > 0)
            {
                criticScore = (int)CalculateCriticScore(product);
            }
            else
            {
                criticScore = -1;
            }

            if (criticReviewAmount >= CriticReviewQuantityThreshold && userReviewAmount >= UserReviewQuantityThreshold)
            {
                superScore = (int)CalculateSuperScore(product, userScore, criticScore);
            }
            else
            {
                superScore = -1;
            }
        }
        private double CalculateUserScore(Product product)
        {
            double maxDiffVotes = 0;
            double minDiffVotes = 999999;
            double totalReviews = 0;

            foreach (var review in product.reviewMatches)
            {
                if (!review.isCritic)
                {
                    maxDiffVotes = GetMaxVoteDiff(review, maxDiffVotes);
                    minDiffVotes = GetMinVoteDiff(review, minDiffVotes);
                }
            }

            foreach (var review in product.reviewMatches)
            {
                double receptionModifier;
                if (minDiffVotes == 0 && maxDiffVotes == 0)
                {
                    receptionModifier = 1;
                }
                else
                {
                    receptionModifier = CalculateReceptionModifier(maxDiffVotes, minDiffVotes, review.positiveReception, review.negativeReception, review);
                }
               
                totalReviews += ((receptionModifier + review.reviewWeight) / 2); //This makes sense
                review.reviewReceptionModifier = receptionModifier;

                if (minDiffVotes == 0 && maxDiffVotes == 0)
                {
                    review.reviewReceptionModifier = 1;
                }
            }



            return (AverageWeightedUserScore(product.reviewMatches, totalReviews) * 100);
        }

        private double CalculateCriticScore(Product product)
        {
            double totalScore = 0;
            double totalReviews = 0;

            foreach (var review in product.reviewMatches)
            {
                if (review.isCritic)
                {
                    totalScore += review.normalizedScore * review.reviewWeight; //This makes sense
                    totalReviews++;
                }
                
            }

            return ((totalScore / totalReviews) * 100);
        }

        private double CalculateSuperScore(Product product, double userScore, double criticScore)
        {
            double productFactor = 4;
            double productAge = (DateTime.Today.Subtract(product.oldestReviewDate).Days) / (double)365;
            double weightedAverageScore = ((criticScore * CriticScoreWeight) + (userScore * UserScoreWeight)) /
                                (CriticScoreWeight + UserScoreWeight);

            if (categoryFactors.ContainsKey(product.Category))
            {
                productFactor = categoryFactors[product.Category];
            }
            
            return  weightedAverageScore * ComputeDecayWeight(productAge, productFactor);
        }

        private double ComputeDecayWeight(double age, double halfPoint)
        {
            // 1/(1 + e^(c * x/b - c))
            double exponent = 5 * age / halfPoint - 5;
            double result = 1 / (1 + Math.Pow(Math.E, exponent));

            return result;
        }

        private void CalculateOldestReviewDate(Product product)
        {
            DateTime oldestReviewDate = DateTime.Now;

            foreach (Review review in product.reviewMatches)
            {
                if (review.ReviewDate < oldestReviewDate)
                {
                    oldestReviewDate = review.ReviewDate;
                }
            }
            product.oldestReviewDate = oldestReviewDate;
        }


        private void CalculateReviewWeight(Product product)
        {
            double productFactor = 4;// default decay weight

            CalculateOldestReviewDate(product);

            // product category factor
            if (categoryFactors.ContainsKey(product.Category))
            {
                productFactor = categoryFactors[product.Category];
            }

            foreach (Review review in product.reviewMatches)
            {
                // review weight (age)
                TimeSpan reviewAge = review.ReviewDate.Subtract(product.oldestReviewDate);
                double reviewAgeInYears = reviewAge.Days/(double) 365;
                review.reviewWeight = ComputeReviewWeight(reviewAgeInYears, productFactor);
            }
        }

        private double ComputeReviewWeight(double age, double categoryFactor)
        {
            double exponent = -2 * age + 2.5;
            double result = 1 - (1 / (1 + 10 * Math.Pow(Math.E, exponent)));

            return result;
        }

        private double NormalizeRating(double rating, double maxRating)
        {
            return (rating/maxRating);
        }

       

        private double AverageWeightedUserScore(List<Review> reviews, double totalReviews)
        {
            double totalScore = 0;
            foreach (var review in reviews)
            {
                if (!review.isCritic)
                {
                    totalScore += (review.normalizedScore * ((review.reviewReceptionModifier + review.reviewWeight) / (double)2));
                }
            }
            return (totalScore / totalReviews);
        }

        private double CalculateReceptionModifier(double maxDiffVotes, double minDiffVotes, double upvotes, double downvotes, Review review)
        {
            return ((upvotes - downvotes) - minDiffVotes) / (maxDiffVotes - minDiffVotes);
        }

        private double GetMaxVoteDiff(Review review, double maxDiffVotes)
        {
            double diff = review.positiveReception - review.negativeReception;
            if (diff > maxDiffVotes)
            {
                return diff;
            }
            else
            {
                return maxDiffVotes;
            }
        }

        private double GetMinVoteDiff(Review review, double minDiffVotes)
        {
            double diff = review.positiveReception - review.negativeReception;
            if (diff < minDiffVotes)
            {
                return diff;
            }
            else
            {
                return minDiffVotes;
            }
        }

        

        private double calculateTimeDecay(DateTime earliestProductReviewDateTime, double superScore)
        {
            double newSuperScore = 0;

            return newSuperScore;
        }

    }
}
