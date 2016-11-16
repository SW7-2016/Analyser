using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Diagnostics;
using analyzer.Products.ProductComponents;
using analyzer.Products.Retailers;
using analyzer.Products.Reviews;

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
        private readonly string connectionString = "server=172.25.23.57;database=crawlerdb;user=analyser;port=3306;password=Analyser23!;";
        private readonly string connectionString2 = "server=172.25.23.57;database=analyserdb;user=analyser;port=3306;password=Analyser23!;";

        public MySqlConnection connection;

        public void DbInitialize(bool isCrawlerDb)
        {
            if (isCrawlerDb)
                connection = new MySqlConnection(connectionString);
            else
                connection = new MySqlConnection(connectionString2);
        }

        public List<Motherboard> GetMotherboardData()
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM Motherboard", connection);
            List<Motherboard> result = new List<Motherboard>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                Motherboard row = new Motherboard("Motherboard", (int)tempResult[0], (string)tempResult[1], (string)tempResult[2], 
                                                (int)tempResult[3], (string)tempResult[4], reader.GetBoolean(5), reader.GetBoolean(6),
                                                reader.GetBoolean(7), reader.GetBoolean(8), reader.GetBoolean(9), (int)tempResult[10], 
                                                (int)tempResult[11], (string)tempResult[12], reader.GetBoolean(13), (string)tempResult[14]);
                                                
                result.Add(row);
            }


            reader.Close();

            return result;
        }

        public List<HardDrive> GetHardDriveData()
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM HardDrive", connection);
            List<HardDrive> result = new List<HardDrive>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                HardDrive row = new HardDrive("HardDrive", (int)tempResult[0], reader.GetBoolean(1), (string)tempResult[2], (string)tempResult[3], 
                                                (string)tempResult[4], (string)tempResult[5], (string)tempResult[6], (string)tempResult[7], 
                                                (string)tempResult[8], (string)tempResult[9], (string)tempResult[10], (string)tempResult[11]);

                result.Add(row);
            }


            reader.Close();

            return result;
        }

        public List<GPU> GetGpuData()
        {
            //MySqlCommand command = new MySqlCommand("SELECT * FROM GPU", connection);
            MySqlCommand command = new MySqlCommand("SELECT DISTINCT * FROM GPU " +
                                                    "NATURAL JOIN(" +
                                                        "SELECT processorManufacturer, chipset, model, architecture, memSize, pciSlots, manufacturer" +
                                                        "FROM GPU " +
                                                        "GROUP BY processorManufacturer, chipset, model, architecture, memSize, pciSlots, manufacturer" +
                                                        "HAVING   COUNT(*) > 1)" +
                                                        " AS t)");
            List<GPU> result = new List<GPU>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {

                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                GPU row = new GPU("GPU", (int)tempResult[0], (string)tempResult[1], (string)tempResult[2], (string)tempResult[3], 
                                    (string)tempResult[4], (string)tempResult[5], (string)tempResult[6], (int)tempResult[7], (string)tempResult[8]);

                result.Add(row);
            }


            reader.Close();

            return result;
        }

        public List<CPU> GetCpuData()
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM CPU", connection);
            List<CPU> result = new List<CPU>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                CPU row = new CPU("CPU", (int)tempResult[0], (string)tempResult[1], (string)tempResult[2], (string)tempResult[3],
                                    (string)tempResult[4], reader.GetBoolean(5), (string)tempResult[6], (string)tempResult[7], 
                                    (int)tempResult[8], (int)tempResult[9], (string)tempResult[10]);

                result.Add(row);
            }


            reader.Close();

            return result;
        }

        public List<Chassis> GetChassisData()
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM Chassis", connection);
            List<Chassis> result = new List<Chassis>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                Chassis row = new Chassis("Chassis", (int)tempResult[0], (string)tempResult[1], reader.GetBoolean(2), reader.GetBoolean(3),
                                            reader.GetBoolean(4), (string)tempResult[5], (string)tempResult[6], (string)tempResult[7],
                                            (string)tempResult[8], (string)tempResult[9], (string)tempResult[10]);

                result.Add(row);
            }


            reader.Close();

            return result;
        }

        public List<PSU> GetPsuData()
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM PSU", connection);
            List<PSU> result = new List<PSU>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                PSU row = new PSU("PSU", (int)tempResult[0], (string)tempResult[1], (string)tempResult[2], reader.GetBoolean(3), (string)tempResult[4], 
                                    (string)tempResult[5], (string)tempResult[6], (string)tempResult[7], (string)tempResult[8]);

                result.Add(row);
            }


            reader.Close();

            return result;
        }

        public List<RAM> GetRamData()
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM RAM", connection);
            List<RAM> result = new List<RAM>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                RAM row = new RAM("RAM", (int)tempResult[0], (string)tempResult[1], (string)tempResult[2], (string)tempResult[3], (string)tempResult[4],
                                    (string)tempResult[5], (string)tempResult[6]);

                result.Add(row);
            }


            reader.Close();

            return result;
        }

        public List<CriticReview> GetCriticReviewData()
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM Review WHERE isCriticReview", connection);
            List<CriticReview> result = new List<CriticReview>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                CriticReview row = new CriticReview((int)tempResult[0], (float)tempResult[4], (float)tempResult[14], reader.GetDateTime(1),
                                    (string)tempResult[13], (string)tempResult[12], (string)tempResult[11]);
                if (!reader.IsDBNull(7) && !reader.IsDBNull(8))
                {
                    row.positiveReception = (int)tempResult[7];
                    row.negativeReception = (int)tempResult[8];
                }
                    
                result.Add(row);
            }

            reader.Close();

            return result;
        }

        public List<UserReview> GetUserReviewData()
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM Review WHERE isCriticReview<>1", connection);
            List<UserReview> result = new List<UserReview>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                UserReview row = new UserReview((int)tempResult[0], (double)tempResult[4], (double)tempResult[14], reader.GetDateTime(2),
                                    (string)tempResult[13], (string)tempResult[12], (string)tempResult[11], reader.GetBoolean(9));
                if (!reader.IsDBNull(7) && !reader.IsDBNull(8))
                {
                    row.positiveReception = (int)tempResult[7];
                    row.negativeReception = (int)tempResult[8];
                }


                result.Add(row);
            }


            reader.Close();

            return result;
        }
    }
}