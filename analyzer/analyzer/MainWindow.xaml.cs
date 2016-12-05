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

        private void RemoveInvalidLinks(ReviewProductLinks reviewProductLinks, ReviewProductLinks actualReviewProductLinks)
        {
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
        }

        private void GetDataTest_bt_Click(object sender, RoutedEventArgs e)
        {

            DBConnect dbConnection = new DBConnect();
            ReviewProductLinks reviewProductLinks = new ReviewProductLinks();
            ReviewProductLinks actualReviewProductLinks = new ReviewProductLinks();

            dbConnection.DbInitialize(true);

            dbConnection.connection.Open();

            #region Add data from crawlerDB

            gpuList = dbConnection.GetGpuData();
            chassisList = dbConnection.GetChassisData();
            cpuList = dbConnection.GetCpuData();
            hardDriveList = dbConnection.GetHardDriveData();
            motherboardList = dbConnection.GetMotherboardData();
            psuList = dbConnection.GetPsuData();
            ramList = dbConnection.GetRamData();
            criticReviewList = dbConnection.GetCriticReviewData();
            userReviewList = dbConnection.GetUserReviewData();

            #endregion

            reviewList.AddRange(criticReviewList);
            reviewList.AddRange(userReviewList);

            foreach (var gpu in cpuList)
            {
                gpu.MatchReviewAndProduct(reviewList, reviewProductLinks);
            }

            RemoveInvalidLinks(reviewProductLinks, actualReviewProductLinks);

            dbConnection.connection.Close();

            /* ||===================================================||
             * ||!! Warning! you are now entering the debug area. !!||
             * ||---------------------------------------------------||
             * ||Here are noting true and everything might be wrong ||
             * ||            Proceed at your own risk               ||
             * ||===================================================||*/

            //Debugging.Debugging.DebugReviewDuplicates(chassisList, cpuList, gpuList, hardDriveList, motherboardList, psuList, ramList);
            //Debugging.Debugging.GetUnlinkedReviews(reviewList, chassisList, cpuList, gpuList, hardDriveList, motherboardList, psuList, ramList);
            Debugging.Debugging.NumberOfReviewForEachProduct(cpuList);
        }
        
        public void AssessScores(List<Product> productList)
        {
            //todo move/instanciate to proper place
            Dictionary<string,double> categoryFactors = new Dictionary<string, double>();
            categoryFactors.Add("GPU", 2);
            categoryFactors.Add("CPU", 3);

            foreach (var product in productList)
            {
                //find oldest _critic_ review to assess product age
                List<Review> reviewList = product.reviewMatches;
                foreach (Review review in product.reviewMatches)
                {
                    double score;

                }
                //compute score
            }
        }
        
        public double ComputeSuperscore(double age, double categoryFactor)
        {

            return 0;
        }

    }
}
