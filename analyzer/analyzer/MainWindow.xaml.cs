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
using analyzer.Score;
using analyzer.Products.DistinctProductList;

namespace analyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetDataTest_bt_Click(object sender, RoutedEventArgs e)
        {
            DistinctProductList<CPU> cpuList; //list of all cpu products, after merging
            DistinctProductList<GPU> gpuList;
            DistinctReviewList<Review> reviewListCpu = new DistinctReviewList<Review>(); //list of all cpu reviews
            DistinctReviewList<Review> reviewListGpu = new DistinctReviewList<Review>();
            ReviewProductLinks reviewProductLinks = new ReviewProductLinks(); //contains the products and reviews which have been linked
            ReviewProductLinks actualReviewProductLinks = new ReviewProductLinks(); //contains linked products and reviews, reviews linking to multiple products removed
            int productsPerThread = 10; //determines the amount of products each thread task should process

            DBConnect dbConnection = new DBConnect(); //create a database connection handler

            #region Add data from crawlerDB
            dbConnection.DbInitialize(true);
            dbConnection.connection.Open();

            //gpuList = dbConnection.GetGpuData();
            cpuList = dbConnection.GetCpuData();

            reviewListCpu.AddDistinctList(dbConnection.GetCriticReviewData("CPU").ToReview());
            reviewListCpu.AddDistinctList(dbConnection.GetUserReviewData("CPU").ToReview());
            reviewListGpu.AddDistinctList(dbConnection.GetCriticReviewData("GPU").ToReview());
            reviewListGpu.AddDistinctList(dbConnection.GetUserReviewData("GPU").ToReview());
  
            dbConnection.connection.Close();
            #endregion

            StartThreads(productsPerThread, cpuList, reviewListCpu); //execute threaded processing for CPUs
            //StartThreads(productsPerThread, gpuList, reviewListGpu);

            while (ThreadingData.semaphore != 0) //wait until all threads are done
            {
                Thread.Sleep(300); //no need for main thread to work while waiting
            }

            foreach (var threadReviewProductLink in ThreadingData.threadProcessedData) //collect each thread's processed data
            {
                reviewProductLinks.productList.AddRange(threadReviewProductLink.productList);
                reviewProductLinks.reviewList.AddRange(threadReviewProductLink.reviewList);
            }

            actualReviewProductLinks = RemoveInvalidLinks(ref reviewProductLinks); //remove invalid links (reviews which link to multiple products)

            #region DebuGZ
            /* ||===================================================||
             * ||!! Warning! you are now entering the debug area. !!||
             * ||---------------------------------------------------||
             * ||Here are noting true and everything might be wrong ||
             * ||            Proceed at your own risk               ||
             * ||===================================================||*/
            //Dictionary<string, bool> helloo = new Dictionary<string, bool>();

            /*gpuList.testPruning = gpuList.testPruning.OrderByDescending(a => a[1]).ToList();
            cpuList.testPruning = cpuList.testPruning.OrderByDescending(a => a[1]).ToList();
            reviewListCpu.testPruning = reviewListCpu.testPruning.OrderByDescending(a => a[1]).ToList();
            reviewListGpu.testPruning = reviewListGpu.testPruning.OrderByDescending(a => a[1]).ToList();*/

            /*foreach (var product in actualReviewProductLinks.productList)
            {
                foreach (var review in product.reviewMatches)
                {
                    if (!helloo.ContainsKey(review.Title))
                    {
                        Debug.WriteLine("");
                        Debug.WriteLine(product.Id + " " + product);
                        Debug.WriteLine(review.Id + " " + review.Title);
                        helloo.Add(review.Title, true);
                    }
                }
            }*/
            //gpuList.NearDublicates();
            //Debugging.Debugging.DebugReviewDuplicates(chassisList, cpuList, gpuList, hardDriveList, motherboardList, psuList, ramList);
            //Debugging.Debugging.GetUnlinkedReviews(reviewListGpu, cpuList, gpuList);
            //Debugging.Debugging.NumberOfReviewForEachProduct(cpuList);
            #endregion

            //Score.Score.AssessProductListScores(cpuList);
            //var scoredProducts = Score.Score.AssessProductListScores(actualReviewProductLinks.productList);

            //SCORE
            foreach (var product in actualReviewProductLinks.productList)
            {
                product.score.CalculateProductScore(product);

            }

            //
            dbConnection.DbInitialize(false);
            dbConnection.connection.Open();

            foreach (Product product in actualReviewProductLinks.productList)
            {
                if (product.score.superScore != -1)
                {
                    product.WriteToDB(dbConnection.connection);
                }
                
                foreach (Review review in product.reviewMatches)
                {
                    review.WriteToDB(dbConnection.connection);
                }
            }
            /*
            cpuList[1].WriteToDB(dbConnection.connection);
            cpuList[1].reviewMatches[0].WriteToDB(dbConnection.connection);
            gpuList[1].WriteToDB(dbConnection.connection);
            gpuList[1].reviewMatches[0].WriteToDB(dbConnection.connection);
            */
            dbConnection.connection.Close();
        }

        public void StartThreads<T>(int productsPerThread, DistinctProductList<T> productList, DistinctReviewList<Review> reviewList) where T : Product
        {
            for (int i = 0; i < productList.Count; i += productsPerThread)
            {
                if (productList.Count - i > productsPerThread) //amount of products left to process is above that which the thread task should process
                {
                    ThreadingData.threadProcessedData.Add(new ReviewProductLinks()); //this specific thread's container for processed data
                    ThreadPool.QueueUserWorkItem(ThreadfunctionProduct<T>, new ThreadingData<T>(ThreadingData.semaphore, productList.GetRange(i, productsPerThread), reviewList));
                    Interlocked.Increment(ref ThreadingData.semaphore); //interlocked ensure atomic increment of semaphore
                }
                else //amount of products left to process is the last batch to process
                {
                    ThreadingData.threadProcessedData.Add(new ReviewProductLinks());
                    ThreadPool.QueueUserWorkItem(ThreadfunctionProduct<T>, new ThreadingData<T>(ThreadingData.semaphore, productList.GetRange(i, productList.Count - i), reviewList));
                    Interlocked.Increment(ref ThreadingData.semaphore);
                    break;
                }

            }
        }

        public void ThreadfunctionProduct<T>(object data) where T : Product
        {
            DistinctProductList<T> productList = ((ThreadingData<T>)data).productList;
            DistinctReviewList<Review> reviewList = ((ThreadingData<T>)data).reviewList;
            ReviewProductLinks processedReviewProductLinks = ThreadingData.threadProcessedData[((ThreadingData<T>)data).id];

            foreach (var product in productList)
            {
                product.MatchReviewAndProduct1(reviewList, productList.stopWord, ref processedReviewProductLinks); //execute linking processing
            }

            Interlocked.Decrement(ref ThreadingData.semaphore);
        }

        private ReviewProductLinks RemoveInvalidLinks(ref ReviewProductLinks reviewProductLinks)
        { //removes links from products and reviews, when a review links to multiple products. 
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
    }
}
