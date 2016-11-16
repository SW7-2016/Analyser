using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using analyzer.Products;
using analyzer.Products.Reviews;
using analyzer.Products.ProductComponents;
using analyzer.GetRawData;

namespace analyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Chassis> chassisList = new List<Chassis>();
        public List<CPU> cpuList = new List<CPU>();
        public List<GPU> gpuList = new List<GPU>();
        public List<HardDrive> hardDriveList = new List<HardDrive>();
        public List<Motherboard> motherboardList = new List<Motherboard>();
        public List<PSU> psuList = new List<PSU>();
        public List<RAM> ramList = new List<RAM>();
        public List<CriticReview> criticReviewList = new List<CriticReview>();
        public List<UserReview> userReviewList = new List<UserReview>();

        public MainWindow()
        {
            InitializeComponent();
        }
        private void GetDataTest_bt_Click(object sender, RoutedEventArgs e)
        {

            DBConnect DBConnection = new DBConnect();

            DBConnection.DbInitialize(true);

            DBConnection.connection.Open();

            #region Add data from crawlerDB

            chassisList = DBConnection.GetChassisData();
            cpuList = DBConnection.GetCpuData();
            gpuList = DBConnection.GetGpuData();
            hardDriveList = DBConnection.GetHardDriveData();
            motherboardList = DBConnection.GetMotherboardData();
            psuList = DBConnection.GetPsuData();
            ramList = DBConnection.GetRamData();
            criticReviewList = DBConnection.GetCriticReviewData();
            userReviewList = DBConnection.GetUserReviewData();


            #endregion







            DBConnection.connection.Close();
        }
    }
}
