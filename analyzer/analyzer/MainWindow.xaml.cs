using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using analyzer.Debugging;
using analyzer.Products.Reviews;
using analyzer.Products.ProductComponents;
using analyzer.GetRawData;
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
        private void GetDataTest_bt_Click(object sender, RoutedEventArgs e)
        {

            DBConnect dbConnection = new DBConnect();

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

            foreach (var gpu in gpuList)
            {
                gpu.MatchReviewAndProduct(reviewList, gpuList);
            }

            dbConnection.connection.Close();

            //Debugging.Debugging.DebugReviewDuplicates(chassisList, cpuList, gpuList, hardDriveList, motherboardList, psuList, ramList);
            Debugging.Debugging.GetUnlinkedReviews(reviewList, chassisList, cpuList, gpuList, hardDriveList, motherboardList, psuList, ramList);
            //TempDebug(); //Remove before commit

        }


        //Remove before commit
        private void TempDebug()
        {
            int largest = 0;
            int id = 0;
            int total = 0;
            int withReviews = 0;

            foreach (var cpu in cpuList)
            {
                if (cpu.reviewMatches.Count > largest)
                {
                    largest = cpu.reviewMatches.Count;
                    id = cpu.Id;
                }

                if (cpu.reviewMatches.Count > 0)
                {
                    Debug.WriteLine("{0} reviews on ID: {1} Model:{2} CpuSeries: {3}", cpu.reviewMatches.Count, cpu.Id, cpu.Model, cpu.CpuSeries);
                    withReviews++;
                }

                total++;
            }

            Debug.WriteLine("");
            Debug.WriteLine("Largest set of reviews is {0} on product id {1}", largest, id);
            Debug.WriteLine("{0} out of {1} have minimum one review linked", withReviews, total);
        }
    }
}
