using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace analyzer.Products.ProductComponents
{
    public class CPU : ComputerComponents
    {
        public CPU(string category, int id, string name, string model, string clock, string maxTurbo, string integratedGpu,
                    bool stockCooler, string manufacturer, string cpuSeries, int logicalCores, int physicalCores, string socket)
            : base(id, category, name)
        {
            Model = model;
            Clock = clock;
            MaxTurbo = maxTurbo;
            IntegratedGpu = integratedGpu;
            StockCooler = stockCooler;
            Manufacturer = manufacturer;
            CpuSeries = cpuSeries;
            LogicalCores = logicalCores;
            PhysicalCores = physicalCores;
            Socket = socket;
        }

        public int PhysicalCores { get; }
        public int LogicalCores { get; }
        public bool StockCooler { get; }
        public string Model { get; }
        public string Clock { get; }
        public string Socket { get; }
        public string MaxTurbo { get; }
        public string IntegratedGpu { get; }
        public string Manufacturer { get; }
        public string CpuSeries { get; }

        public List<CPU> GetCpuData()
        {
            crawlerConnection.Open();

            MySqlCommand command =
                new MySqlCommand(
                    "SELECT Product.ProductID, Product.name, CPU.model, CPU.clock, CPU.maxTurbo, CPU.integratedGpu, " +
                    "CPU.stockCooler, CPU.manufacturer, CPU.cpuSeries, CPU.logicalCores, CPU.physicalCores, CPU.socket " +
                    "FROM Product, CPU " +
                    "WHERE Product.ProductID = CPU.ProductID", crawlerConnection);
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
            List<CPU> result = new List<CPU>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                CPU row = new CPU("CPU", (int)tempResult[0], (string)tempResult[1], (string)tempResult[2],
                    (string)tempResult[3],
                    (string)tempResult[4], (string)tempResult[5], reader.GetBoolean(6), (string)tempResult[7],
                    (string)tempResult[8], (int)tempResult[9], (int)tempResult[10], (string)tempResult[11]);

                result.Add(row);
            }


            reader.Close();
            crawlerConnection.Close();

            return result;
        }

        public override string ToString()
        {
            return $"{nameof(PhysicalCores)}: {PhysicalCores}, {nameof(LogicalCores)}: {LogicalCores}, {nameof(StockCooler)}: {StockCooler}, {nameof(Model)}: {Model}, {nameof(Clock)}: {Clock}, {nameof(Socket)}: {Socket}, {nameof(MaxTurbo)}: {MaxTurbo}, {nameof(IntegratedGpu)}: {IntegratedGpu}, {nameof(Manufacturer)}: {Manufacturer}, {nameof(CpuSeries)}: {CpuSeries}";
        }


    }

}