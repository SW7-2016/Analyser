using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace analyzer.Products.ProductComponents
{
    public class PSU : ComputerComponents
    {
        public PSU(string category, int id, string name, string power, string formFactor, bool modular, string width, 
                    string depth, string height, string weight, string brand) 
            : base(id, category, name)
        {
            Power = power;
            FormFactor = formFactor;
            Modular = modular;
            Width = width;
            Depth = depth;
            Height = height;
            Weight = weight;
            Brand = brand;
        }

        public string Power { get; }
        public string FormFactor { get; }
        public string Brand { get; }
        public string Height { get; }
        public string Depth { get; }
        public string Width { get; }
        public string Weight { get; }
        public bool Modular { get; }

        public List<PSU> GetPsuData()
        {
            crawlerConnection.Open();

            MySqlCommand command =
                new MySqlCommand("SELECT Product.ProductID, Product.name, PSU.power, PSU.formFactor, PSU.modular, " +
                                 "PSU.width, PSU.depth, PSU.height, PSU.weight, PSU.brand " +
                                 "FROM Product, PSU " +
                                 "WHERE Product.ProductID = PSU.ProductID", crawlerConnection);
            List<PSU> result = new List<PSU>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                PSU row = new PSU("PSU", (int)tempResult[0], (string)tempResult[1], (string)tempResult[2],
                    (string)tempResult[3], reader.GetBoolean(4),
                    (string)tempResult[5], (string)tempResult[6], (string)tempResult[7], (string)tempResult[8],
                    (string)tempResult[9]);

                result.Add(row);
            }


            reader.Close();
            crawlerConnection.Close();

            return result;
        }
    }
}
