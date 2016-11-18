﻿using System.Collections.Generic;
using System.Windows;
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



            dbConnection.connection.Close();
        }
    }
}
