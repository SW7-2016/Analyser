﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters;
using analyzer.Products.Reviews;
using MySql.Data.MySqlClient;

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

        public override void MatchReviewAndProduct(List<Review> reviewList, Dictionary<string, bool> stopWords, ref ReviewProductLinks reviewProductLinks)
        {
            List<string> productTokens = SplitStringToTokens(Model.ToLower() + " " + CpuSeries.ToLower());
            productTokens = RemoveRestrictedTokens(productTokens, stopWords);

            foreach (var review in reviewList)
            {
                if (review.Category.ToLower() != "cpu")
                    continue;

                List<string> reviewTitleNoStopWords = RemoveRestrictedTokens(SplitStringToTokens(review.Title.ToLower()), stopWords);
                string concatenatedReviewTitle = "";

                foreach (var token in reviewTitleNoStopWords)
                {
                    concatenatedReviewTitle += token;
                }

                if (CompareReviewTitleWithProductStrings(concatenatedReviewTitle, productTokens))
                {
                    //Debug.WriteLine("");
                    //Debug.WriteLine(this.Id + " " + this);
                    //Debug.WriteLine(review.Id + " " + review.Title);
                    //add review id to product
                    reviewMatches.Add(review);
                    review.linkedProducts.Add(this);

                    if (!reviewProductLinks.productList.Contains(this))
                    {
                        reviewProductLinks.productList.Add(this);
                    }

                    if (!reviewProductLinks.reviewList.Contains(review))
                    {
                        reviewProductLinks.reviewList.Add(review);
                    }
                }
            }
        }

        public override void MatchReviewAndProduct1(List<Review> reviewList, Dictionary<string, bool> stopWords, ref ReviewProductLinks reviewProductLinks)
        {
            string productStrings = Model.ToLower() + " " + CpuSeries.ToLower();
            List<string> productTokens = RemoveRestrictedTokens(SplitStringToTokens(productStrings), stopWords);


            foreach (var review in reviewList)
            {
                if (review.Category.ToLower() != "cpu")
                    continue;

                if (CompareReviewTitleWithProductStrings1(review.Title.ToLower(), productTokens, stopWords))
                {
                   //add review id to product
                    reviewMatches.Add(review);
                    review.linkedProducts.Add(this);

                    if (!reviewProductLinks.productList.Contains(this))
                    {
                        reviewProductLinks.productList.Add(this);
                    }

                    if (!reviewProductLinks.reviewList.Contains(review))
                    {
                        reviewProductLinks.reviewList.Add(review);
                    }
                }
            }
        }

        public override string ToString()
        {
            //return $"{nameof(PhysicalCores)}: {PhysicalCores}, {nameof(LogicalCores)}: {LogicalCores}, {nameof(StockCooler)}: {StockCooler}, {nameof(Model)}: {Model}, {nameof(Clock)}: {Clock}, {nameof(Socket)}: {Socket}, {nameof(MaxTurbo)}: {MaxTurbo}, {nameof(IntegratedGpu)}: {IntegratedGpu}, {nameof(Manufacturer)}: {Manufacturer}, {nameof(CpuSeries)}: {CpuSeries}";
            return $"{nameof(Model)}: {Model}, {nameof(Manufacturer)}: {Manufacturer}, {nameof(CpuSeries)}: {CpuSeries}";
        }

        public void WriteToDB(MySqlConnection connection)
        {
            MySqlCommand command = new MySqlCommand("INSERT INTO cpu" +
                                                   "(id,name,model,clock,max_turbo,integrated_gpu,stock_cooler,manufacturer,cpu_series,logical_cores,physical_cores,socket,superscore,avg_critic_score,avg_user_score,oldest_review_date,newest_review_date)" +
                                                   "VALUES(@ProductID, @name, @model, @clock, @max_turbo, @integrated_gpu, @stock_cooler, @manufacturer, @cpu_series, @logical_cores, @physical_cores, @socket, @superscore, @avg_critic_score, @avg_user_score, @oldest_review_date, @newest_review_date)",
               connection);
            command.Parameters.AddWithValue("@ProductID", Id);
            command.Parameters.AddWithValue("@name", Name);
            command.Parameters.AddWithValue("@model", Model);
            command.Parameters.AddWithValue("@clock", Clock);
            command.Parameters.AddWithValue("@max_turbo", MaxTurbo);
            command.Parameters.AddWithValue("@integrated_gpu", IntegratedGpu);
            command.Parameters.AddWithValue("@stock_cooler", StockCooler);
            command.Parameters.AddWithValue("@manufacturer", Manufacturer);
            command.Parameters.AddWithValue("@cpu_series", CpuSeries);
            command.Parameters.AddWithValue("@logical_cores", LogicalCores);
            command.Parameters.AddWithValue("@physical_cores", PhysicalCores);
            command.Parameters.AddWithValue("@socket", Socket);
            command.Parameters.AddWithValue("@superscore", superScore);
            command.Parameters.AddWithValue("@avg_critic_score", criticScore);
            command.Parameters.AddWithValue("@avg_user_score", userScore);
            command.Parameters.AddWithValue("@oldest_review_date", oldestReviewDate);
            command.Parameters.AddWithValue("@newest_review_date", newestReviewDate);

            command.ExecuteNonQuery();
        }

    }

}