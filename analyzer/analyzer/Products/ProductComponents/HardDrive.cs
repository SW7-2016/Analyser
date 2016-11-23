using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace analyzer.Products.ProductComponents
{
    public class HardDrive : ComputerComponents
    {
        public HardDrive(string category, int id, string name, bool isInternal, string type, string formFactor, string capacity, string cacheSize, 
                        string transferRate, string brand, string sata, string height, string depth, string width) 
            : base(id, category, name)
        {
            IsInternal = isInternal;
            Type = type;
            FormFactor = formFactor;
            Capacity = capacity;
            CacheSize = cacheSize;
            TransferRate = transferRate;
            Brand = brand;
            Sata = sata;
            Height = height;
            Depth = depth;
            Width = width;
        }

        public bool IsInternal { get; }
        public string Type { get; }
        public string FormFactor { get; }
        public string Capacity { get; }
        public string CacheSize { get; }
        public string TransferRate { get; }
        public string Brand { get; }
        public string Sata { get; }
        public string Height { get; }
        public string Depth { get; }
        public string Width { get; }

        public List<HardDrive> GetHardDriveData()
        {
            crawlerConnection.Open();

            MySqlCommand command =
                new MySqlCommand(
                    "SELECT Product.ProductID, Product.name, HardDrive.isInternal, HardDrive.type, HardDrive.formFactor, " +
                    "HardDrive.capacity, HardDrive.cacheSize, HardDrive.transferRate, HardDrive.brand, HardDrive.sata, " +
                    "HardDrive.height, HardDrive.depth, HardDrive.width " +
                    "FROM Product, HardDrive " +
                    "WHERE Product.ProductID = HardDrive.ProductID", crawlerConnection);

            List<HardDrive> result = new List<HardDrive>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                HardDrive row = new HardDrive("HardDrive", (int)tempResult[0], (string)tempResult[1],
                    reader.GetBoolean(2), (string)tempResult[3],
                    (string)tempResult[4], (string)tempResult[5], (string)tempResult[6], (string)tempResult[7],
                    (string)tempResult[8], (string)tempResult[9], (string)tempResult[10], (string)tempResult[11],
                    (string)tempResult[12]);

                result.Add(row);
            }


            reader.Close();
            crawlerConnection.Close();

            return result;
        }
    }
}
