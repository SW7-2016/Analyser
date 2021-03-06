﻿using System.Collections.Generic;
using analyzer.Products.Reviews;
using MySql.Data.MySqlClient;
using analyzer.Products.DistinctProductList;

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

        public override void WriteToDB(MySqlConnection dbConnection)
        {
            throw new System.NotImplementedException();
        }
    }
}
