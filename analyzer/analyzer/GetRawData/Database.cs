using System.Collections.Generic;
using MySql.Data.MySqlClient;
using analyzer.Products.ProductComponents;
using analyzer.Products.Reviews;
using analyzer.Products.DistinctProductList.types;
using System.Text.RegularExpressions;
using analyzer.Products;

namespace analyzer.GetRawData
{
    class DBConnect
    {
        /*
        MySqlCommand command = new MySqlCommand("SELECT * FROM CPU WHERE url=@url", connection);
        command.Parameters.AddWithValue("@url", url);

            MySqlDataReader reader = command.ExecuteReader();

        reader.Read();

            int result = (int)reader.GetValue(0);

        reader.Close();*/

        private readonly string connectionString =
            "server=172.25.23.57;database=crawlerdb;user=analyser;port=3306;password=Analyser23!;";

        private readonly string connectionString2 =
            "server=172.25.23.57;database=analyserdb;user=analyser;port=3306;password=Analyser23!;";

        public MySqlConnection connection;

        public void DbInitialize(bool isCrawlerDb)
        {
            if (isCrawlerDb)
                connection = new MySqlConnection(connectionString);
            else
                connection = new MySqlConnection(connectionString2);
        }

        public DistinctProductList<Motherboard> GetMotherboardData()
        {
            MySqlCommand command =
                new MySqlCommand(
                    "SELECT Product.ProductID, Product.name, Motherboard.formFactor, Motherboard.cpuType, " +
                    "Motherboard.cpuCount, Motherboard.socket, Motherboard.netcard, Motherboard.soundCard, " +
                    "Motherboard.multiGpu, Motherboard.crossfire, Motherboard.sli, Motherboard.maxMem, " +
                    "Motherboard.memSlots, Motherboard.memType, Motherboard.graphicsCard, Motherboard.chipset " +
                    "FROM Product, Motherboard " +
                    "WHERE Product.ProductID = Motherboard.ProductID", connection);


            DistinctProductList<Motherboard> result = new DistinctProductList<Motherboard>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                Motherboard row = new Motherboard("Motherboard", (int) tempResult[0], (string) tempResult[1],
                    (string) tempResult[2], (string) tempResult[3],
                    (int) tempResult[4], (string) tempResult[5], reader.GetBoolean(6), reader.GetBoolean(7),
                    reader.GetBoolean(8), reader.GetBoolean(9), reader.GetBoolean(10), (int) tempResult[11],
                    (int) tempResult[12], (string) tempResult[13], reader.GetBoolean(14), (string) tempResult[15]);

                result.Add(row);
            }


            reader.Close();

            return result;
        }

        public DistinctProductList<HardDrive> GetHardDriveData()
        {
            MySqlCommand command =
                new MySqlCommand(
                    "SELECT Product.ProductID, Product.name, HardDrive.isInternal, HardDrive.type, HardDrive.formFactor, " +
                    "HardDrive.capacity, HardDrive.cacheSize, HardDrive.transferRate, HardDrive.brand, HardDrive.sata, " +
                    "HardDrive.height, HardDrive.depth, HardDrive.width " +
                    "FROM Product, HardDrive " +
                    "WHERE Product.ProductID = HardDrive.ProductID", connection);

            DistinctProductList<HardDrive> result = new DistinctProductList<HardDrive>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                HardDrive row = new HardDrive("HardDrive", (int) tempResult[0], (string) tempResult[1],
                    reader.GetBoolean(2), (string) tempResult[3],
                    (string) tempResult[4], (string) tempResult[5], (string) tempResult[6], (string) tempResult[7],
                    (string) tempResult[8], (string) tempResult[9], (string) tempResult[10], (string) tempResult[11],
                    (string) tempResult[12]);

                result.Add(row);
            }


            reader.Close();

            return result;
        }

        public DistinctProductList<GPU> GetGpuData()
        {
            MySqlCommand command =
                new MySqlCommand(
                    "SELECT Product.ProductID, Product.name, GPU.processorManufacturer, GPU.chipset, GPU.graphicsProcessor, " +
                    "GPU.architecture, GPU.cooling, GPU.memSize, GPU.pciSlots, GPU.manufacturer, GPU.model, GPU.boostedClock " +
                    "FROM Product, GPU " +
                    "WHERE Product.ProductID = GPU.ProductID AND GPU.manufacturer != \"\" AND GPU.graphicsProcessor != \"\" AND GPU.model != \"\"", connection);
            /*MySqlCommand command = new MySqlCommand(
            SELECT processorManufacturer, chipset, model, architecture, cooling, memSize, pciSlots, manufacturer, count(*) 
            FROM GPU
            GROUP BY processorManufacturer, chipset, model, architecture, cooling, memSize, pciSlots, manufacturer);
            */
            DistinctProductList<GPU> result = new DistinctProductList<GPU>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                GPU row = new GPU("GPU", (int) tempResult[0], (string) tempResult[1], (string) tempResult[2],
                    (string) tempResult[3], (string) tempResult[4], (string) tempResult[5], (string) tempResult[6], 
                    (string) tempResult[7], (int) tempResult[8], (string) tempResult[9], 
                    (string) tempResult[10],//model
                    (string) tempResult[11]//boosted clock
                    );

                result.Add(row);
            }


            reader.Close();

            return result;
        }

        public DistinctProductList<CPU> GetCpuData()
        {
            MySqlCommand command =
                new MySqlCommand(
                    "SELECT Product.ProductID, Product.name, CPU.model, CPU.clock, CPU.maxTurbo, CPU.integratedGpu, " +
                    "CPU.stockCooler, CPU.manufacturer, CPU.cpuSeries, CPU.logicalCores, CPU.physicalCores, CPU.socket " +
                    "FROM Product, CPU " +
                    "WHERE Product.ProductID = CPU.ProductID AND CPU.Model != \"\" AND CPU.cpuSeries != \"\"", connection);
            /*
            CREATE TABLE MergedProduct2 AS SELECT * FROM Product;

            update MergedProduct2, CPU SET name = REPLACE(name, ', Tray', '') WHERE MergedProduct2.ProductID = CPU.ProductID;
            update MergedProduct2, CPU SET name = REPLACE(name, ', Box', '') WHERE MergedProduct2.ProductID = CPU.ProductID;
            update MergedProduct2, CPU SET name = REPLACE(name, ' Tray', '') WHERE MergedProduct2.ProductID = CPU.ProductID;
            update MergedProduct2, CPU SET name = REPLACE(name, ' Box', '') WHERE MergedProduct2.ProductID = CPU.ProductID;
            update MergedProduct2, CPU SET name = REPLACE(name, ' (Inteled)', '') WHERE MergedProduct2.ProductID = CPU.ProductID;
            update MergedProduct2, CPU SET name = REPLACE(name, ' (OEM/bakke)', '') WHERE MergedProduct2.ProductID = CPU.ProductID;
            update MergedProduct2, CPU SET name = REPLACE(name, ' (AMD Processor in a (PIB))', '') WHERE MergedProduct2.ProductID = CPU.ProductID;


            SELECT *
            FROM MergedProduct2, CPU
            where MergedProduct2.ProductID = CPU.ProductID;

            drop table MergedProduct2;
             */
            DistinctProductList<CPU> result = new DistinctProductList<CPU>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                CPU row = new CPU("CPU", (int) tempResult[0], (string) tempResult[1], (string) tempResult[2],
                    (string) tempResult[3],
                    (string) tempResult[4], (string) tempResult[5], reader.GetBoolean(6), (string) tempResult[7],
                    (string) tempResult[8], (int) tempResult[9], (int) tempResult[10], (string) tempResult[11]);

                Match model = Regex.Match(row.Model, "\\d\\d\\d" );

                if (model.Success)
                {
                    result.Add(row);
                }
            }


            reader.Close();

            return result;
        }

        public DistinctProductList<Chassis> GetChassisData()
        {
            MySqlCommand command =
                new MySqlCommand(
                    "SELECT Product.ProductID, Product.name, Chassis.type, Chassis.atx, Chassis.miniAtx, Chassis.miniItx, " +
                    "Chassis.fans, Chassis.brand, Chassis.height, Chassis.width, Chassis.depth, Chassis.weight " +
                    "FROM Product, Chassis " +
                    "WHERE Product.ProductID = Chassis.ProductID", connection);
            DistinctProductList<Chassis> result = new DistinctProductList<Chassis>();

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                Chassis row = new Chassis("Chassis", (int) tempResult[0], (string) tempResult[1], (string) tempResult[2],
                    reader.GetBoolean(3),
                    reader.GetBoolean(4), reader.GetBoolean(5), (string) tempResult[6], (string) tempResult[7],
                    (string) tempResult[8], (string) tempResult[9], (string) tempResult[10], (string) tempResult[11]);

                result.Add(row);
            }


            reader.Close();

            return result;
        }

        public DistinctProductList<PSU> GetPsuData()
        {
            MySqlCommand command =
                new MySqlCommand("SELECT Product.ProductID, Product.name, PSU.power, PSU.formFactor, PSU.modular, " +
                                 "PSU.width, PSU.depth, PSU.height, PSU.weight, PSU.brand " +
                                 "FROM Product, PSU " +
                                 "WHERE Product.ProductID = PSU.ProductID", connection);
            DistinctProductList<PSU> result = new DistinctProductList<PSU>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                PSU row = new PSU("PSU", (int) tempResult[0], (string) tempResult[1], (string) tempResult[2],
                    (string) tempResult[3], reader.GetBoolean(4),
                    (string) tempResult[5], (string) tempResult[6], (string) tempResult[7], (string) tempResult[8],
                    (string) tempResult[9]);

                result.Add(row);
            }


            reader.Close();

            return result;
        }

        public DistinctProductList<RAM> GetRamData()
        {
            MySqlCommand command =
                new MySqlCommand("SELECT Product.ProductID, Product.name, RAM.type, RAM.capacity, RAM.speed, " +
                                 "RAM.technology, RAM.formFactor, RAM.casLatens " +
                                 "FROM Product, RAM " +
                                 "WHERE Product.ProductID = RAM.ProductID", connection);
            DistinctProductList<RAM> result = new DistinctProductList<RAM>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                RAM row = new RAM("RAM", (int) tempResult[0], (string) tempResult[1], (string) tempResult[2],
                    (string) tempResult[3],
                    (string) tempResult[4], (string) tempResult[5], (string) tempResult[6], (string) tempResult[7]);

                result.Add(row);
            }


            reader.Close();

            return result;
        }

        public List<CriticReview> GetCriticReviewData(string category)
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM Review WHERE isCriticReview AND productType = \"" + category + "\"", connection);
        
            List<CriticReview> result = new List<CriticReview>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                CriticReview row = new CriticReview((int) tempResult[0], (float) tempResult[4], (float) tempResult[14],
                    reader.GetDateTime(1),
                    (string) tempResult[13], //title
                    (string) tempResult[12], //url
                    (string) tempResult[11], //category
                    (string) tempResult[3],//content
                    (string) tempResult[6]//author
                    );
                if (!reader.IsDBNull(7) && !reader.IsDBNull(8))
                {
                    row.positiveReception = (int) tempResult[7];
                    row.negativeReception = (int) tempResult[8];
                }

                result.Add(row);
            }

            reader.Close();

            return result;
        }

        public List<UserReview> GetUserReviewData(string category)
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM Review WHERE isCriticReview<>1 AND productType = \"" + category + "\"", connection);
            List<UserReview> result = new List<UserReview>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                UserReview row = new UserReview((int) tempResult[0], (float) tempResult[4], (float) tempResult[14],
                    reader.GetDateTime(1),
                    (string) tempResult[13], (string) tempResult[12], (string) tempResult[11], reader.GetBoolean(9),
                    (string)tempResult[3],//content
                    (string)tempResult[6]//author
                    );
                if (!reader.IsDBNull(7) && !reader.IsDBNull(8))
                {
                    row.positiveReception = (int) tempResult[7];
                    row.negativeReception = (int) tempResult[8];
                }


                result.Add(row);
            }


            reader.Close();

            return result;
        }
        
       }
}