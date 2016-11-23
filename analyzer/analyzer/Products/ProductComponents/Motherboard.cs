using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace analyzer.Products.ProductComponents
{
    public class Motherboard : ComputerComponents
    {
        public Motherboard(string category, int id, string name, string formFactor, string cpuType, int cpuCount, string socket, 
                            bool netCard, bool soundCard, bool multiGpu, bool crossfire, bool sli, int maxMem, 
                            int memSlots, string memType, bool supportIntegratedGraphicsCard, string chipset) 
            : base(id, category, name)
        {
            FormFactor = formFactor;
            CpuType = cpuType;
            CpuCount = cpuCount;
            Socket = socket;
            NetCard = netCard;
            SoundCard = soundCard;
            MultiGpu = multiGpu;
            Crossfire = crossfire;
            Sli = sli;
            MaxMem = maxMem;
            MemSlots = memSlots;
            MemType = memType;
            SupportIntegratedGraphicsCard = supportIntegratedGraphicsCard;
            Chipset = chipset;
        }

        public string FormFactor { get; }
        public string Chipset { get; }
        public string CpuType { get; }
        public string Socket { get; }
        public string MemType { get; }
        public bool NetCard { get; }
        public bool SoundCard { get; }
        public bool MultiGpu { get; }
        public bool SupportIntegratedGraphicsCard { get; }
        public bool Sli { get; }
        public bool Crossfire { get; }
        public int MemSlots { get; }
        public int MaxMem { get; }
        public int CpuCount { get; }

        public List<Motherboard> GetMotherboardData()
        {
            crawlerConnection.Open();

            MySqlCommand command =
                new MySqlCommand(
                    "SELECT Product.ProductID, Product.name, Motherboard.formFactor, Motherboard.cpuType, " +
                    "Motherboard.cpuCount, Motherboard.socket, Motherboard.netcard, Motherboard.soundCard, " +
                    "Motherboard.multiGpu, Motherboard.crossfire, Motherboard.sli, Motherboard.maxMem, " +
                    "Motherboard.memSlots, Motherboard.memType, Motherboard.graphicsCard, Motherboard.chipset " +
                    "FROM Product, Motherboard " +
                    "WHERE Product.ProductID = Motherboard.ProductID", crawlerConnection);


            List<Motherboard> result = new List<Motherboard>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                Motherboard row = new Motherboard("Motherboard", (int)tempResult[0], (string)tempResult[1],
                    (string)tempResult[2], (string)tempResult[3],
                    (int)tempResult[4], (string)tempResult[5], reader.GetBoolean(6), reader.GetBoolean(7),
                    reader.GetBoolean(8), reader.GetBoolean(9), reader.GetBoolean(10), (int)tempResult[11],
                    (int)tempResult[12], (string)tempResult[13], reader.GetBoolean(14), (string)tempResult[15]);

                result.Add(row);
            }


            reader.Close();
            crawlerConnection.Close();

            return result;
        }
    }
}
