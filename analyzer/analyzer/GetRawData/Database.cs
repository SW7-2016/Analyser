using System.Collections.Generic;
using MySql.Data.MySqlClient;
using analyzer.Products.ProductComponents;
using analyzer.Products.Reviews;

namespace analyzer.GetRawData
{
    public class DBConnect
    {
        public MySqlConnection crawlerConnection = new MySqlConnection("server=172.25.23.57;database=crawlerdb;user=analyser;port=3306;password=Analyser23!;");
        public MySqlConnection analyserConnection = new MySqlConnection("server=172.25.23.57;database=analyserdb;user=analyser;port=3306;password=Analyser23!;");

    }
}