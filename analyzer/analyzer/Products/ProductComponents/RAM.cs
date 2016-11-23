using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace analyzer.Products.ProductComponents
{
    public class RAM : ComputerComponents
    {
        public RAM(string category, int id, string name, string type, string capacity, string speed, 
                    string technology, string formFactor, string casLatency) 
            : base(id, category, name)
        {
            Type = type;
            Capacity = capacity;
            Speed = speed;
            Technology = technology;
            FormFactor = formFactor;
            CasLatency = casLatency;
        }

        public string Type { get; }
        public string Capacity { get; }
        public string Speed { get; }
        public string Technology { get; }
        public string FormFactor { get; }
        public string CasLatency { get; }

        public List<RAM> GetRamData()
        {
            crawlerConnection.Open();

            MySqlCommand command =
                new MySqlCommand("SELECT Product.ProductID, Product.name, RAM.type, RAM.capacity, RAM.speed, " +
                                 "RAM.technology, RAM.formFactor, RAM.casLatens " +
                                 "FROM Product, RAM " +
                                 "WHERE Product.ProductID = RAM.ProductID", crawlerConnection);
            List<RAM> result = new List<RAM>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                RAM row = new RAM("RAM", (int)tempResult[0], (string)tempResult[1], (string)tempResult[2],
                    (string)tempResult[3],
                    (string)tempResult[4], (string)tempResult[5], (string)tempResult[6], (string)tempResult[7]);

                result.Add(row);
            }


            reader.Close();
            crawlerConnection.Close();

            return result;
        }
    }
}
