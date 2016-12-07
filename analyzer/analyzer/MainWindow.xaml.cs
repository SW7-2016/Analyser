using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using analyzer.Debugging;
using analyzer.Products.Reviews;
using analyzer.Products.ProductComponents;
using analyzer.GetRawData;
using analyzer.Products;
using analyzer.Products.DistinctProductList.types;
using System.Threading;
using analyzer.Threading;

namespace analyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int semaphore = 0;
        List<ReviewProductLinks> threadProcessedData = new List<ReviewProductLinks>();

        public MainWindow()
        {
            InitializeComponent();
        }



        private void GetDataTest_bt_Click(object sender, RoutedEventArgs e)
        {
            DistinctProductList<CPU> cpuList = new DistinctProductList<CPU>();
            DistinctProductList<GPU> gpuList = new DistinctProductList<GPU>();
            List<CriticReview> criticReviewListCpu = new List<CriticReview>();
            List<CriticReview> criticReviewListGpu = new List<CriticReview>();
            List<UserReview> userReviewListCpu = new List<UserReview>();
            List<UserReview> userReviewListGpu = new List<UserReview>();
            List<Review> reviewListCpu = new List<Review>();
            List<Review> reviewListGpu = new List<Review>();

            DBConnect dbConnection = new DBConnect();

            #region Add data from crawlerDB
            dbConnection.DbInitialize(true);
            dbConnection.connection.Open();

            gpuList = dbConnection.GetGpuData();
            //chassisList = dbConnection.GetChassisData();
            cpuList = dbConnection.GetCpuData();
            //hardDriveList = dbConnection.GetHardDriveData();
            //motherboardList = dbConnection.GetMotherboardData();
            //psuList = dbConnection.GetPsuData();
            //ramList = dbConnection.GetRamData();
            criticReviewListCpu = dbConnection.GetCriticReviewData("CPU");
            criticReviewListGpu = dbConnection.GetCriticReviewData("GPU");
            userReviewListGpu = dbConnection.GetUserReviewData("GPU");
            userReviewListCpu = dbConnection.GetUserReviewData("CPU");

            reviewListCpu.AddRange(criticReviewListCpu);
            reviewListCpu.AddRange(userReviewListCpu);
            reviewListGpu.AddRange(criticReviewListGpu);
            reviewListGpu.AddRange(userReviewListGpu);

            dbConnection.connection.Close();
            #endregion

            int productsPerThread = 200;

            StartThreads(productsPerThread, cpuList, reviewListCpu);
            StartThreads(productsPerThread, gpuList, reviewListGpu);

            while (semaphore != 0)
            {
                Thread.Sleep(500);
            }

            ReviewProductLinks threadReviewProductLinks = new ReviewProductLinks();
            
            foreach (var threadededreviewProductLink in threadProcessedData)
            {
                threadReviewProductLinks.productList.AddRange(threadededreviewProductLink.productList);
                threadReviewProductLinks.reviewList.AddRange(threadededreviewProductLink.reviewList);
            }
            ReviewProductLinks actualThreadReviewProductLinks = RemoveInvalidLinks(threadReviewProductLinks);

            #region DebuGZ
            /* ||===================================================||
             * ||!! Warning! you are now entering the debug area. !!||
             * ||---------------------------------------------------||
             * ||Here are noting true and everything might be wrong ||
             * ||            Proceed at your own risk               ||
             * ||===================================================||*/

            /*foreach (var product in actualReviewProductLinks.productList)
            {
                foreach (var review in product.reviewMatches)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine(product.Id + " " + product);
                    Debug.WriteLine(review.Id + " " + review.Title);
                }
            }*/

            //Debugging.Debugging.DebugReviewDuplicates(chassisList, cpuList, gpuList, hardDriveList, motherboardList, psuList, ramList);
            //Debugging.Debugging.GetUnlinkedReviews(reviewList, chassisList, cpuList, gpuList, hardDriveList, motherboardList, psuList, ramList);
            //Debugging.Debugging.NumberOfReviewForEachProduct(cpuList);
            #endregion
        }

        public void StartThreads<T>(int productsPerThread, DistinctProductList<T> productList, List<Review> reviewList) where T : Product
        {
            for (int i = 0; i < productList.Count; i += productsPerThread)
            {
                if (productList.Count - i > productsPerThread)
                {
                    threadProcessedData.Add(new ReviewProductLinks());
                    ThreadPool.QueueUserWorkItem(ThreadfunctionProduct<T>, new ThreadingData<T>(semaphore, productList.GetRange(i, productsPerThread), reviewList));
                    Interlocked.Increment(ref semaphore);
                }
                else
                {
                    threadProcessedData.Add(new ReviewProductLinks());
                    ThreadPool.QueueUserWorkItem(ThreadfunctionProduct<T>, new ThreadingData<T>(semaphore, productList.GetRange(i, productList.Count - i), reviewList));
                    Interlocked.Increment(ref semaphore);
                    break;
                }

            }
        }
        public void ThreadfunctionProduct<T>(object data) where T : Product
        {
            DistinctProductList<T> hii = ((ThreadingData<T>)data).productList;
            List<Review> reviewList = ((ThreadingData<T>)data).reviewList;

            ReviewProductLinks threadededreviewProductLinks = threadProcessedData[((ThreadingData<T>)data).id];

            foreach (var product in hii)
            {
                product.MatchReviewAndProduct1(reviewList, hii.stopWord, ref threadededreviewProductLinks);
            }
            Interlocked.Decrement(ref semaphore);
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



            /* ||===================================================||
             * ||!! Warning! you are now entering the debug area. !!||
             * ||---------------------------------------------------||
             * ||Here are noting true and everything might be wrong ||
             * ||            Proceed at your own risk               ||
             * ||===================================================||*/

            //Debugging.Debugging.DebugReviewDuplicates(chassisList, cpuList, gpuList, hardDriveList, motherboardList, psuList, ramList);
            //Debugging.Debugging.GetUnlinkedReviews(reviewList, chassisList, cpuList, gpuList, hardDriveList, motherboardList, psuList, ramList);
            //Debugging.Debugging.NumberOfReviewForEachProduct(cpuList);

            Score.Score.AssessProductListScores(cpuList);
            Score.Score.AssessProductListScores(gpuList);
        }
        
    }
}
