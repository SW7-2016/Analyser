using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace analyzer.Products.ProductComponents
{
    public class Motherboard : ComputerComponents
    {
        public Motherboard(string category, int id, string formFactor, string cpuType, int cpuCount, string socket, 
                            bool netCard, bool soundCard, bool multiGpu, bool crossfire, bool sli, int maxMem, 
                            int memSlots, string memType, bool supportIntegratedGraphicsCard, string chipset) 
            : base(id, category)
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
    }
}
