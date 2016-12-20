using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using analyzer.Products;
using analyzer.Products.ProductComponents;
using analyzer.Products.Reviews;

namespace analyzer.Scores
{
    public class Score
    {
        
        private static double CriticReviewQuantityThreshold = 1; //minimum amount of critic reviews needed, before using them for superscore
        private static double UserReviewQuantityThreshold = 10; //minimum amount of user reviews needed, before using them for superscore

        private double weightedUserScore; //user score to be used for the superscore
        private double weightedCriticScore; //critic score to be used for the superscore
        public double superScore; // the superscore presented in the presentation module
        public double avgCriticScore; //an average critic score, to be shown in the presentation module
        public double avgUserScore; //an average user score, to be shown in the presentation module

        public void CalculateProductScore(Product product)
        {
            double criticReviewAmount = 0; //how many critic reviews does the current product have
            double userReviewAmount = 0; //how many user reviews does the current product have

            //Calculate and normalizes review scores
            foreach (var review in product.reviewMatches)
            {
                review.normalizedScore = NormalizeRating(review.Rating, review.MaxRating);
                //counts reviews
                if (review.isCritic && review.Rating != -1)
                {
                    criticReviewAmount++;
                }
                else if (!review.isCritic && review.Rating != -1)
                {
                    userReviewAmount++;
                }
            }
            //calculates the weights of each review
            CalculateReviewWeight(product);

            //UserScore
            if (userReviewAmount > 0)
            {
                weightedUserScore = (int) CalculateUserScore(product, userReviewAmount);
            }
            else
            {
                weightedUserScore = -1; //userscore is not used
            }
            //CriticScore
            if (criticReviewAmount > 0)
            {
                weightedCriticScore = (int) CalculateCriticScore(product, criticReviewAmount);
            }
            else
            {
                weightedCriticScore = -1; //critic score is not used
            }
            //SuperScore
            if (criticReviewAmount >= CriticReviewQuantityThreshold || userReviewAmount >= UserReviewQuantityThreshold)
            {
                superScore = (int) CalculateSuperScore(product, weightedUserScore, weightedCriticScore);
            }
            else
            {
                superScore = -1; //no superscore is calculated - the product will not be added to the presentation module
            }
        }

        //calculates the average and weighted userscore, returns the weighted score
        private double CalculateUserScore(Product product, double numberOfUserReviews)
        {
            double maxDiffVotes = 0; //Used to represent the best/highest amount of good votes of a review (goodvote - badvote)
            double minDiffVotes = 999999; //Used to represent the worst/lowest amount of good votes of a review (goodvote - badvote)
            double totalReviews = 0; //"total reviews" equals all reviewWeights and reception modifiers combined, used for normalization
            double totalUserScore = 0; //combined normalized userscore for product

            //finds max and min vote amount, and the all normalized user scores added up
            foreach (var review in product.reviewMatches)
            {
                if (!review.isCritic)
                {
                    maxDiffVotes = GetMaxVoteDiff(review, maxDiffVotes);
                    minDiffVotes = GetMinVoteDiff(review, minDiffVotes);
                    totalUserScore += review.normalizedScore;
                }
            }
            //if there is more than 0 reviews, calculate average
            if (numberOfUserReviews > 0)
            {
                avgUserScore = ((totalUserScore / numberOfUserReviews) * 100);
            }
            

            foreach (var review in product.reviewMatches)
            {
                double receptionModifier;
                if (minDiffVotes == 0 && maxDiffVotes == 0) //if not votes on any review
                {
                    receptionModifier = 1;
                }
                else
                {
                    receptionModifier = CalculateReceptionModifier(maxDiffVotes, minDiffVotes, review.positiveReception,
                        review.negativeReception, review);
                }
                //"total reviews" equals all reviewWeights and reception modifiers combined
                //used for normalization
                totalReviews += ((receptionModifier + review.reviewWeight)/2); 
                review.reviewReceptionModifier = receptionModifier;

                if (minDiffVotes == 0 && maxDiffVotes == 0)
                {
                    review.reviewReceptionModifier = 1;
                }
            }

            return (AverageWeightedUserScore(product.reviewMatches, totalReviews)*100);
        }

        //calculates the average and weighted critic score, returns the weighted score
        private double CalculateCriticScore(Product product, double totalReviews)
        {
            double totalScoreWeighted = 0;
            double totalScore = 0;

            foreach (var review in product.reviewMatches)
            {
                if (review.isCritic)
                {
                    totalScoreWeighted += review.normalizedScore*review.reviewWeight;
                    totalScore += review.normalizedScore;
                }
            }

            avgCriticScore = ((totalScore/totalReviews) * 100);

            return ((totalScoreWeighted/totalReviews)*100);
        }

        //calculates superscore
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

        //calculates the time decay weight for product
        private double ComputeDecayWeight(double age, double halfPoint)
        {
            // 1/(1 + e^(c * x/b - c))
            double exponent = 5*age/halfPoint - 5;
            double result = 1/(1 + Math.Pow(Math.E, exponent));

            return result;
        }



        //finds newest and oldest review date
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

        //calculates review weights based on time, for each review
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

        //computes review weight for a review
        private double ComputeReviewWeight(double age, double categoryFactor)
        {
            double exponent = -2*age + 2.5;
            double result = 1 - (0.5/(1 + 10*Math.Pow(Math.E, exponent))) + 0.01;

            return result;
        }

        //normalizes a rating
        private double NormalizeRating(double rating, double maxRating)
        {
            return (rating/maxRating);
        }

        //Calculate averageweighteduserscore
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

        //calculates weight based on review reception
        private double CalculateReceptionModifier(double maxDiffVotes, double minDiffVotes, double upvotes,
            double downvotes, Review review)
        {
            return ((upvotes - downvotes) - minDiffVotes)/(maxDiffVotes - minDiffVotes);
        }

        //checks current max diff vote against current review vote diff and returns the largest
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

        //checks current min diff vote against current review vote diff and returns the smallest
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
    }
}