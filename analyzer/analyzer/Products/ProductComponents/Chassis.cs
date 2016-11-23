using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace analyzer.Products.ProductComponents
{
    public class Chassis : ComputerComponents
    {
        public Chassis(string category, int id, string name, string type, bool atx, bool miniAtx, bool miniItx,
                    string fans, string brand, string height, string width, string depth, string weight)
            : base(id, category, name)
        {
            Type = type;
            Atx = atx;
            MiniAtx = miniAtx;
            MiniItx = miniItx;
            Fans = fans;
            Brand = brand;
            Height = height;
            Width = width;
            Depth = depth;
            Weight = weight;
        }

        public bool Atx { get; }
        public bool MiniAtx { get; }
        public bool MiniItx { get; }
        public string Type { get; }
        public string Brand { get; }
        public string Height { get; }
        public string Width { get; }
        public string Weight { get; }
        public string Depth { get; }
        public string Fans { get; }

        public List<Chassis> GetChassisData()
        {
            crawlerConnection.Open();

            MySqlCommand command =
                new MySqlCommand(
                    "SELECT Product.ProductID, Product.name, Chassis.type, Chassis.atx, Chassis.miniAtx, Chassis.miniItx, " +
                    "Chassis.fans, Chassis.brand, Chassis.height, Chassis.width, Chassis.depth, Chassis.weight " +
                    "FROM Product, Chassis " +
                    "WHERE Product.ProductID = Chassis.ProductID", crawlerConnection);
            List<Chassis> result = new List<Chassis>();

            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                Chassis row = new Chassis("Chassis", (int)tempResult[0], (string)tempResult[1], (string)tempResult[2],
                    reader.GetBoolean(3),
                    reader.GetBoolean(4), reader.GetBoolean(5), (string)tempResult[6], (string)tempResult[7],
                    (string)tempResult[8], (string)tempResult[9], (string)tempResult[10], (string)tempResult[11]);

                result.Add(row);
            }


            reader.Close();
            crawlerConnection.Close();

            return result;
        }
    }
}
