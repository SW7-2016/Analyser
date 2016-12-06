using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using analyzer.Debugging;
using analyzer.Products.Reviews;
using analyzer.Products.ProductComponents;
using analyzer.GetRawData;
using analyzer.Products;
using analyzer.Products.DistinctProductList.types;

namespace analyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DistinctProductList<Chassis> chassisList = new DistinctProductList<Chassis>();
        public DistinctProductList<CPU> cpuList = new DistinctProductList<CPU>();
        public DistinctProductList<GPU> gpuList = new DistinctProductList<GPU>();
        public DistinctProductList<HardDrive> hardDriveList = new DistinctProductList<HardDrive>();
        public DistinctProductList<Motherboard> motherboardList = new DistinctProductList<Motherboard>();
        public DistinctProductList<PSU> psuList = new DistinctProductList<PSU>();
        public DistinctProductList<RAM> ramList = new DistinctProductList<RAM>();
        public List<CriticReview> criticReviewList = new List<CriticReview>();
        public List<UserReview> userReviewList = new List<UserReview>();
        public List<Review> reviewList = new List<Review>();


        public MainWindow()
        {
            InitializeComponent();
        }

        private ReviewProductLinks RemoveInvalidLinks(ReviewProductLinks reviewProductLinks)
        {
            ReviewProductLinks actualReviewProductLinks = new ReviewProductLinks();
            foreach (var review in reviewProductLinks.reviewList)
            {
                if (!(review.linkedProducts.Count > 1))
                {
                    actualReviewProductLinks.reviewList.Add(review);
                }
                else
                {
                    foreach (var id in review.linkedProducts)
                    {
                        foreach (var product in reviewProductLinks.productList)
                        {
                            if (product.Id == id.Id)
                            {
                                product.reviewMatches.Remove(review);
                            }
                        }
                    }
                }
            }

            foreach (var product in reviewProductLinks.productList)
            {
                if (product.reviewMatches.Count > 0)
                {
                    actualReviewProductLinks.productList.Add(product);
                }
            }

            return actualReviewProductLinks;
        }

        private void GetDataTest_bt_Click(object sender, RoutedEventArgs e)
        {

            DBConnect dbConnection = new DBConnect();
            ReviewProductLinks reviewProductLinks = new ReviewProductLinks();
            ReviewProductLinks actualReviewProductLinks;

            dbConnection.DbInitialize(true);

            dbConnection.connection.Open();

            #region Add data from crawlerDB

            //gpuList = dbConnection.GetGpuData();
            //chassisList = dbConnection.GetChassisData();
            cpuList = dbConnection.GetCpuData();
            //hardDriveList = dbConnection.GetHardDriveData();
            //motherboardList = dbConnection.GetMotherboardData();
            //psuList = dbConnection.GetPsuData();
            //ramList = dbConnection.GetRamData();
            criticReviewList = dbConnection.GetCriticReviewData();
            userReviewList = dbConnection.GetUserReviewData();

            #endregion

            reviewList.AddRange(criticReviewList);
            reviewList.AddRange(userReviewList);

            foreach (var gpu in cpuList)
            {
                gpu.MatchReviewAndProduct(reviewList, cpuList.stopWord, ref reviewProductLinks);
            }

            actualReviewProductLinks = RemoveInvalidLinks(reviewProductLinks);

            foreach (var product in actualReviewProductLinks.productList)
            {
                foreach (var review in product.reviewMatches)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine(product.Id + " " + product);
                    Debug.WriteLine(review.Id + " " + review.Title);
                }
            }

            dbConnection.connection.Close();

            /* ||===================================================||
             * ||!! Warning! you are now entering the debug area. !!||
             * ||---------------------------------------------------||
             * ||Here are noting true and everything might be wrong ||
             * ||            Proceed at your own risk               ||
             * ||===================================================||*/

            //Debugging.Debugging.DebugReviewDuplicates(chassisList, cpuList, gpuList, hardDriveList, motherboardList, psuList, ramList);
            Debugging.Debugging.GetUnlinkedReviews(reviewList, chassisList, cpuList, gpuList, hardDriveList, motherboardList, psuList, ramList);
            //Debugging.Debugging.NumberOfReviewForEachProduct(cpuList);
        }
        
        public void AssessScores(List<Product> productList)
        {
            //todo move/instanciate to proper place
            Dictionary<string,double> categoryFactors = new Dictionary<string, double>();
            categoryFactors.Add("GPU", 2);
            categoryFactors.Add("CPU", 3);

            foreach (var product in productList)
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
                    double normalizedScore = review.Rating/review.MaxRating;

                    //compute score
                    if (isCriticReview)
                    {
                        criticReviewNumerator += normalizedScore*review.reviewWeight;
                        criticReviewDenominator += review.reviewWeight;
                    }
                    else
                    {
                        userReviewNumerator += normalizedScore * review.reviewWeight;
                        userReviewDenominator += review.reviewWeight;
                    }
                }

                double criticScore = criticReviewNumerator/criticReviewDenominator;

                //superscore = critic*weigth + user*weight / sum weight
            }
        }
        
        public double ComputeReviewWeight(double age, double categoryFactor)
        {
            //\frac{1}{(1 + 10 * e^{-2 * x + 2.5}) + 1}
            double exponent = -2 * age + 2.5;
            double result = 1/(1 + 10 * Math.Pow(Math.E, exponent)) + 1;

            return result;
        }

    }
}
