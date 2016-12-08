using System.Collections.Generic;
using MySql.Data.MySqlClient;
using analyzer.Products.ProductComponents;
using analyzer.Products.Reviews;
using analyzer.Products.DistinctProductList.types;
using System.Text.RegularExpressions;
using analyzer.Products;
using analyzer.Products.DistinctProductList;

namespace analyzer.GetRawData
{
    class DBConnect
    {
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

        public DistinctReviewList<CriticReview> GetCriticReviewData(string category)
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM Review WHERE isCriticReview AND productType = \"" + category + "\"", connection);

            DistinctReviewList<CriticReview> result = new DistinctReviewList<CriticReview>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                CriticReview row = new CriticReview((int) tempResult[0], reader.GetBoolean(10), (float) tempResult[4], (float) tempResult[14],
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

        public DistinctReviewList<UserReview> GetUserReviewData(string category)
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM Review WHERE isCriticReview<>1 AND productType = \"" + category + "\"", connection);
            DistinctReviewList<UserReview> result = new DistinctReviewList<UserReview>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                UserReview row = new UserReview((int) tempResult[0], reader.GetBoolean(10), (float) tempResult[4], (float) tempResult[14],
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
        
        public void WriteReviewToDB(int productID, Review review)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO review" +
                                                   "(id,product_id,date,is_critic,url,title,author,rating,content)" +
                                                   "VALUES(@id, @product_id, @date, @is_critic, @url, @title, @author, @rating, @content)",
               connection);

            command.Parameters.AddWithValue("@id", productID);
            command.Parameters.AddWithValue("@product_id", productID);//todo what?
            command.Parameters.AddWithValue("@date", review.ReviewDate);
            command.Parameters.AddWithValue("@is_critic", review.isCritic);
            command.Parameters.AddWithValue("@url", review.Url);
            command.Parameters.AddWithValue("@title", review.Title);
            command.Parameters.AddWithValue("@author", review.Author);//todo must be set in Review
            command.Parameters.AddWithValue("@rating", review.Rating);
            command.Parameters.AddWithValue("@content", review.Content);//todo must be set in Review

            command.ExecuteNonQuery();
        }

        public void WriteGpuToDB(GPU product)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO gpu" +
                                                   "(product_id,name,model,processor_manufacturer,manufacturer,graphic_processor,mem_size,boosted_clock,superscore,avg_critic_score,avg_user_score,oldest_review_date,newest_review_date)" +
                                                   "VALUES(@ProductID, @name, @model, @processor_manufacturer, @manufacturer, @graphic_processor, @mem_size, @boosted_clock, @superscore, @avg_critic_score, @avg_user_score, @oldest_review_date, @newest_review_date)",
               connection);
            command.Parameters.AddWithValue("@ProductID", product.Id);
            command.Parameters.AddWithValue("@name", product.Name);
            command.Parameters.AddWithValue("@model", product.Model);
            command.Parameters.AddWithValue("@processor_manufacturer", product.ProcessorManufacturer);
            command.Parameters.AddWithValue("@manufacturer", product.Manufacturer);
            command.Parameters.AddWithValue("@graphic_processor", product.GraphicsProcessor);
            command.Parameters.AddWithValue("@mem_size", product.MemSize);
            command.Parameters.AddWithValue("@boosted_clock", product);//todo get boosted clock somehow
            command.Parameters.AddWithValue("@superscore", product.superScore);
            command.Parameters.AddWithValue("@avg_critic_score", product.criticScore);
            command.Parameters.AddWithValue("@avg_user_score", product.userScore);
            command.Parameters.AddWithValue("@oldest_review_date", product.oldestReviewDate);
            command.Parameters.AddWithValue("@newest_review_date", product.newestReviewDate);

            command.ExecuteNonQuery();
        }

        public void WriteCpuToDB(CPU product)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO gpu" +
                                                   "(id,name,model,clock,max_turbo,integrated_gpu,stock_cooler,manufacturer,cpu_series,logical_cores,physical_cores,socket,superscore,avg_critic_score,avg_user_score,oldest_review_date,newest_review_date)" +
                                                   "VALUES(@ProductID, @name, @model, @clock, @max_turbo, @integrated_gpu, @stock_cooler, @manufacturer, @cpu_series, @logical_cores, @physical_cores, @socket, @superscore, @avg_critic_score, @avg_user_score, @oldest_review_date, @newest_review_date)",
               connection);
            command.Parameters.AddWithValue("@ProductID", product.Id);
            command.Parameters.AddWithValue("@name", product.Name);
            command.Parameters.AddWithValue("@model", product.Model);
            command.Parameters.AddWithValue("@clock", product.Clock);
            command.Parameters.AddWithValue("@max_turbo", product.MaxTurbo);
            command.Parameters.AddWithValue("@integrated_gpu", product.IntegratedGpu);
            command.Parameters.AddWithValue("@stock_cooler", product.StockCooler);
            command.Parameters.AddWithValue("@manufacturer", product.Manufacturer);
            command.Parameters.AddWithValue("@cpu_series", product.CpuSeries);
            command.Parameters.AddWithValue("@logical_cores", product.LogicalCores);
            command.Parameters.AddWithValue("@physical_cores", product.PhysicalCores);
            command.Parameters.AddWithValue("@socket", product.Socket);
            command.Parameters.AddWithValue("@superscore", product.superScore);
            command.Parameters.AddWithValue("@avg_critic_score", product.criticScore);
            command.Parameters.AddWithValue("@avg_user_score", product.userScore);
            command.Parameters.AddWithValue("@oldest_review_date", product.oldestReviewDate);
            command.Parameters.AddWithValue("@newest_review_date", product.newestReviewDate);

            command.ExecuteNonQuery();
        }
    }
}