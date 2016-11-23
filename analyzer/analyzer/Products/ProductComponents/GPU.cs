using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace analyzer.Products.ProductComponents
{
    public class GPU : ComputerComponents
    {
        public GPU(string category, int id, string name, string processorManufacturer, string chipset, string model, 
                    string architecture, string cooling, string memSize, int pciSlots, string manufacturer) 
            : base(id, category, name)
        {
            ProcessorManufacturer = processorManufacturer;
            Chipset = chipset;
            Model = model;
            Architecture = architecture;
            Cooling = cooling;
            MemSize = memSize;
            Manufacturer = manufacturer;
            PciSlots = pciSlots;
        }

        public int PciSlots { get; }
        public string ProcessorManufacturer { get; }
        public string Chipset { get; }
        public string Model { get; }
        public string Architecture { get; }
        public string Cooling { get; }
        public string MemSize { get; }
        public string Manufacturer { get; }

        public List<GPU> GetGpuData()
        {
            crawlerConnection.Open();

            MySqlCommand command =
                new MySqlCommand(
                    "SELECT Product.ProductID, Product.name, GPU.processorManufacturer, GPU.chipset, GPU.model, " +
                    "GPU.architecture, GPU.cooling, GPU.memSize, GPU.pciSlots, GPU.manufacturer " +
                    "FROM Product, GPU " +
                    "WHERE Product.ProductID = GPU.ProductID", crawlerConnection);
            /*MySqlCommand command = new MySqlCommand(
            SELECT processorManufacturer, chipset, model, architecture, cooling, memSize, pciSlots, manufacturer, count(*) 
            FROM GPU
            GROUP BY processorManufacturer, chipset, model, architecture, cooling, memSize, pciSlots, manufacturer);
            */
            List<GPU> result = new List<GPU>();
            MySqlDataReader reader = command.ExecuteReader();
            int i;

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                GPU row = new GPU("GPU", (int)tempResult[0], (string)tempResult[1], (string)tempResult[2],
                    (string)tempResult[3], (string)tempResult[4],
                    (string)tempResult[5], (string)tempResult[6], (string)tempResult[7], (int)tempResult[8],
                    (string)tempResult[9]);

                result.Add(row);
            }


            reader.Close();
            crawlerConnection.Close();

            return result;
        }
    }
}
