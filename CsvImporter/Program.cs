using AutoMapper;
using CsvImporter.Helpers;
using FileHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using CsvImporter.Helpers.ObjectDataReader;
using Microsoft.Data.SqlClient;

namespace CsvImporter
{
    public class Program
    {
        public static bool Exited;
        public static DateTime TestStart;
        public static int ProcessedPoints;

        static void Main(string[] args)
        {
            var memoryLogger = new Thread(Logging); memoryLogger.Start();

            ImporterFileHelper();
        }

        public static void ImporterFileHelper()
        {
            var config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<RecordClass, Item>()
                );

            var mapper = new Mapper(config);

            var engine = new FileHelperEngine<RecordClass>();
            var records = engine.ReadFile("C:\\Users\\Emiliano Elicegui\\Stock (1).csv");

            List<Item> items = mapper.Map<List<Item>>(records);

            try
            {

                using (var context = new DBContext())
                {
                    context.Database.BeginTransaction();

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(context.Database.GetDbConnection().ConnectionString))
                    {
                        bulkCopy.DestinationTableName = "dbo.Items";

                        try
                        {
                            // Write from the source to the destination.
                            var reader = items.AsDataReader();

                            SqlBulkCopyColumnMapping PointOfSale =
                            new SqlBulkCopyColumnMapping("PointOfSale", "PointOfSale");
                            bulkCopy.ColumnMappings.Add(PointOfSale);

                            SqlBulkCopyColumnMapping Product =
                            new SqlBulkCopyColumnMapping("Product", "Product");
                            bulkCopy.ColumnMappings.Add(Product);

                            SqlBulkCopyColumnMapping Date =
                            new SqlBulkCopyColumnMapping("Date", "Date");
                            bulkCopy.ColumnMappings.Add(Date);

                            SqlBulkCopyColumnMapping Stock =
                            new SqlBulkCopyColumnMapping("Stock", "Stock");
                            bulkCopy.ColumnMappings.Add(Stock);

                            bulkCopy.BulkCopyTimeout = 0;

                            bulkCopy.WriteToServer(reader);

                            context.Database.CommitTransaction();

                            Console.WriteLine($"Saving list ({items.Count()}) items correct");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            context.Database.RollbackTransaction();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving items, details: {ex.Message}");
            }
            finally
            {
                Exited = true;
            }
        }

        public static void Logging()
        {
            var lastProcessedPoints = 0;
            while (!Exited)
            {
                Thread.Sleep(60 * 1000);
                var lapsed = (DateTime.Now - TestStart);
                var currentUsage = (GC.GetTotalMemory(false) / 1024 / 1024);
                var processedDelta = Math.Round((ProcessedPoints - lastProcessedPoints) * 2.0 / 1000);
                lastProcessedPoints = ProcessedPoints;

                Console.WriteLine("Time Elapsed {0}, Memory Used: {1}Mb, Aprox KPoints/Sec: {2}", lapsed, currentUsage, processedDelta);
            }
        }
    }
}
