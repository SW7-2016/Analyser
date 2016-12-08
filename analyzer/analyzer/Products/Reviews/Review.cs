using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using analyzer.GetRawData;
using MySql.Data.MySqlClient;

namespace analyzer.Products.Reviews
{
    public abstract class Review
    {
        public List<string> pros;
        public List<string> cons;
        public List<Product> linkedProducts;
        public int positiveReception;
        public int negativeReception;
        public double productAgeAtReviewTime;
        public double credibility;
        public double reviewWeight;
        public double normalizedScore;
        public bool isCritic;
        public double reviewReceptionModifier;

        protected Review(int id, bool isCriticReview, double rating, double maxRating, DateTime date, string title, string url, string category, string content, string author)
        {
            Id = id;
            Category = category;
            Url = url;
            Title = title;
            ReviewDate = date;
            Rating = rating;
            MaxRating = maxRating;
            linkedProducts = new List<Product>();
            Content = content;
            Author = author;
            isCritic = isCriticReview;

        }

        public int Id { get; }
        public double Rating { get; }
        public double MaxRating { get; }
        public DateTime ReviewDate { get; }
        public string Url { get; }
        public string Title { get; }
        public string Category { get; }
        public string Content { get; }
        public string Author { get; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Rating)}: {Rating}, {nameof(ReviewDate)}: {ReviewDate}, {nameof(Url)}: {Url}, {nameof(Title)}: {Title}, {nameof(Category)}: {Category}";
        }


        public void WriteToDB(MySqlConnection connection)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO review" +
                                                   "(id,product_id,date,is_critic,url,title,author,rating,content)" +
                                                   "VALUES(@id, @product_id, @date, @is_critic, @url, @title, @author, @rating, @content)",
               connection);

            command.Parameters.AddWithValue("@id", Id);
            command.Parameters.AddWithValue("@product_id", linkedProducts[0].Id);
            command.Parameters.AddWithValue("@date", ReviewDate);
            command.Parameters.AddWithValue("@is_critic", isCritic);
            command.Parameters.AddWithValue("@url", Url);
            command.Parameters.AddWithValue("@title", Title);
            command.Parameters.AddWithValue("@author", Author);
            command.Parameters.AddWithValue("@rating", Rating);
            command.Parameters.AddWithValue("@content", Content);

            command.ExecuteNonQuery();
        }
    }
}