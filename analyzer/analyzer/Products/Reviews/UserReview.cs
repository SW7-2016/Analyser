using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace analyzer.Products.Reviews
{
    public class UserReview : Review
    {
        public UserReview(int id, double rating, double maxRating, DateTime date, string title, string url, string category, bool verifiedPurchase)
                    : base(id, rating, maxRating, date, title, url, category)
        {
            VerifiedPurchase = verifiedPurchase;
        }

        public bool VerifiedPurchase { get; }

        public List<UserReview> GetUserReviewData()
        {
            crawlerConnection.Open();

            MySqlCommand command = new MySqlCommand("SELECT * FROM Review WHERE isCriticReview<>1", crawlerConnection);
            List<UserReview> result = new List<UserReview>();
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] tempResult = new object[reader.FieldCount];
                reader.GetValues(tempResult);

                UserReview row = new UserReview((int)tempResult[0], (float)tempResult[4], (float)tempResult[14],
                    reader.GetDateTime(2),
                    (string)tempResult[13], (string)tempResult[12], (string)tempResult[11], reader.GetBoolean(9));
                if (!reader.IsDBNull(7) && !reader.IsDBNull(8))
                {
                    row.positiveReception = (int)tempResult[7];
                    row.negativeReception = (int)tempResult[8];
                }


                //result.Add(row);
            }


            reader.Close();
            crawlerConnection.Close();

            return result;
        }
    }
}