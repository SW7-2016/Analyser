using System.Collections.Generic;

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
        public List<Chassis> chassisList = new List<Chassis>();
        public DistinctProductList<CPU> cpuList = new DistinctProductList<CPU>();
        public List<GPU> gpuList = new List<GPU>();
        public List<HardDrive> hardDriveList = new List<HardDrive>();
        public List<Motherboard> motherboardList = new List<Motherboard>();
        public List<PSU> psuList = new List<PSU>();
        public List<RAM> ramList = new List<RAM>();
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

            foreach (var cpu in cpuList)
            {
                cpu.MatchReviewAndProduct(reviewList, cpuList);
            }

            dbConnection.connection.Close();

            //Debugging.Debugging.DebugReviewDuplicates(chassisList, cpuList, gpuList, hardDriveList, motherboardList, psuList, ramList);
            //Debugging.Debugging.GetUnlinkedReviews(reviewList, chassisList, cpuList, gpuList, hardDriveList, motherboardList, psuList, ramList);
            
        }
    }
}
