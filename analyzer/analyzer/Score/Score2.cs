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
    public class Score2
    {
        
        private static double CriticReviewQuantityThreshold = 1;
        private static double UserReviewQuantityThreshold = 10;

        private double weightedUserScore;
        private double weightedCriticScore;
        public double superScore;
        public double avgCriticScore;
        public double avgUserScore;

        public void CalculateProductScore(Product product)
        {
            int criticReviewAmount = 0;
            int userReviewAmount = 0;

            //Calculate and normalizes revies scores
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

            CalculateReviewWeight(product);

            //UserScore
            if (userReviewAmount > 0)
            {
                weightedUserScore = (int) CalculateUserScore(product);
            }
            else
            {
                weightedUserScore = -1;
            }
            //CriticScore
            if (criticReviewAmount > 0)
            {
                weightedCriticScore = (int) CalculateCriticScore(product);
            }
            else
            {
                weightedCriticScore = -1;
            }
            //SuperScore
            if (criticReviewAmount >= CriticReviewQuantityThreshold || userReviewAmount >= UserReviewQuantityThreshold)
            {
                superScore = (int) CalculateSuperScore(product, weightedUserScore, weightedCriticScore);
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
            double totalUserScore = 0;
            double numberOfUserReviews = 0;

            foreach (var review in product.reviewMatches)
            {
                if (!review.isCritic)
                {
                    maxDiffVotes = GetMaxVoteDiff(review, maxDiffVotes);
                    minDiffVotes = GetMinVoteDiff(review, minDiffVotes);
                    totalUserScore += review.normalizedScore;
                    numberOfUserReviews++;
                }
            }

            if (numberOfUserReviews > 0)
            {
                avgUserScore = ((totalUserScore / numberOfUserReviews) * 100);
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
                    receptionModifier = CalculateReceptionModifier(maxDiffVotes, minDiffVotes, review.positiveReception,
                        review.negativeReception, review);
                }

                totalReviews += ((receptionModifier + review.reviewWeight)/2); //This makes sense
                review.reviewReceptionModifier = receptionModifier;

                if (minDiffVotes == 0 && maxDiffVotes == 0)
                {
                    review.reviewReceptionModifier = 1;
                }
            }


            return (AverageWeightedUserScore(product.reviewMatches, totalReviews)*100);
        }

        private double CalculateCriticScore(Product product)
        {
            double totalScoreWeighted = 0;
            double totalScore = 0;
            double totalReviews = 0;

            foreach (var review in product.reviewMatches)
            {
                if (review.isCritic)
                {
                    totalScoreWeighted += review.normalizedScore*review.reviewWeight; //This makes sense
                    totalScore += review.normalizedScore;
                    totalReviews++;
                }
            }

            avgCriticScore = ((totalScore/totalReviews) * 100);

            return ((totalScoreWeighted/totalReviews)*100);
        }

        private double CalculateSuperScore(Product product, double userScore, double criticScore)
        {
            double UserScoreWeight = 1;
            double CriticScoreWeight = 2;

            if (criticScore == -1)
            {
                CriticScoreWeight = 0;
            }
            if (userScore == -1)
            {
                UserScoreWeight = 0;
            }

            double productAge = (DateTime.Today.Subtract(product.oldestReviewDate).Days)/(double) 365;
            double weightedAverageScore = ((criticScore*CriticScoreWeight) + (userScore*UserScoreWeight))/
                                          (CriticScoreWeight + UserScoreWeight);

            return weightedAverageScore*ComputeDecayWeight(productAge, product.productFactor);
        }

        private double ComputeDecayWeight(double age, double halfPoint)
        {
            // 1/(1 + e^(c * x/b - c))
            double exponent = 5*age/halfPoint - 5;
            double result = 1/(1 + Math.Pow(Math.E, exponent));

            return result;
        }



        //calculates newest and oldest review date
        private void CalculateExtremeReviewDates(Product product)
        {
            DateTime oldestReviewDate = DateTime.Now;
            DateTime newestReviewDate = new DateTime(1990, 1, 1);

            foreach (Review review in product.reviewMatches)
            {
                if (review.ReviewDate < oldestReviewDate)
                {
                    oldestReviewDate = review.ReviewDate;
                }
                if (review.ReviewDate > newestReviewDate)
                {
                    newestReviewDate = review.ReviewDate;
                }
                    
            }
            product.oldestReviewDate = oldestReviewDate;
            product.newestReviewDate = newestReviewDate;
        }


        private void CalculateReviewWeight(Product product)
        {
            CalculateExtremeReviewDates(product);

            foreach (Review review in product.reviewMatches)
            {
                // review weight (age)
                TimeSpan reviewAge = DateTime.Now.Subtract(review.ReviewDate);
                double reviewAgeInYears = reviewAge.Days/(double) 365;
                review.reviewWeight = ComputeReviewWeight(reviewAgeInYears, product.productFactor);
            }
        }

        private double ComputeReviewWeight(double age, double categoryFactor)
        {
            double exponent = -2*age + 2.5;
            double result = 1 - (0.5/(1 + 10*Math.Pow(Math.E, exponent))) + 0.01;

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
                    totalScore += (review.normalizedScore*
                                   ((review.reviewReceptionModifier + review.reviewWeight)/(double) 2));
                }
            }
            return (totalScore/totalReviews);
        }

        private double CalculateReceptionModifier(double maxDiffVotes, double minDiffVotes, double upvotes,
            double downvotes, Review review)
        {
            return ((upvotes - downvotes) - minDiffVotes)/(maxDiffVotes - minDiffVotes);
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