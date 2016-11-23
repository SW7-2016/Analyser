using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace analyzer.Products.Reviews
{
    public class CriticReview : Review
    {
        public CriticReview(int id, double rating, double maxRating, DateTime date, string title, string url, string category) 
                    : base(id, rating, maxRating, date, title, url, category)
        {
        }

        public List<CriticReview> GetCriticReviewData()
        {
            crawlerConnection.Open();

            MySqlCommand command = new MySqlCommand("SELECT * FROM Review WHERE isCriticReview", crawlerConnection);
            List<CriticReview> result = new List<CriticReview>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                CriticReview row = new CriticReview((int)tempResult[0], (float)tempResult[4], (float)tempResult[14],
                    reader.GetDateTime(1),
                    (string)tempResult[13], (string)tempResult[12], (string)tempResult[11]);
                if (!reader.IsDBNull(7) && !reader.IsDBNull(8))
                {
                    row.positiveReception = (int)tempResult[7];
                    row.negativeReception = (int)tempResult[8];
                }

                result.Add(row);
            }

            reader.Close();
            crawlerConnection.Close();

            return result;
        }
    }
}