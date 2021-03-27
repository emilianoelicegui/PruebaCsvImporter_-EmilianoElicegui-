using CsvImporter.Helpers;
using FileHelpers;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace CsvImporter
{
    public class Program
    {
        public static bool Exited;
        public static long StartingMb;
        public static long PointsCount;
        public static DateTime TestStart;
        public static int ProcessedPoints;

        static void Main(string[] args)
        {
            //var memoryLogger = new Thread(Logging); memoryLogger.Start();

            //ImporterCSV();

            ImporterFileHelper();
        }

        public static void ImporterFileHelper()
        {
            var engine = new FileHelperEngine<RecordClass>();
            var records = engine.ReadFile("C:\\Users\\Emiliano Elicegui\\Stock (1).csv");

            foreach (var record in records)
            {
                Console.WriteLine(record.PointOfSale);
                Console.WriteLine(record.Product);
                Console.WriteLine(record.Date.ToString("dd/MM/yyyy"));
                Console.WriteLine(record.Stock.ToString());
            }
        }
        public static void ImporterCSV()
        {
            try
            {
                var listItems = new List<Item>();

                //read file CSV
                var lines = File.ReadLines("C:\\Users\\Emiliano Elicegui\\Stock (1).csv");

                //validate columns in CSV
                if (lines.First() != "PointOfSale;Product;Date;Stock")
                {
                    throw new Exception($"Format invalid columns CSV");
                }

                var sount = lines.Count();

                //read rows
                for (int i = 1; i < lines.Count(); i++)
                {
                    string[] fields = lines.ElementAt(i).Split(";");

                    //create items 
                    try
                    {
                        var item = new Item
                        {
                            PointOfSale = fields[0].Trim(),
                            Product = fields[1].Trim(),
                            Date = Convert.ToDateTime(fields[2].Trim()),
                            Stock = Convert.ToInt16(fields[3].Trim())
                        };

                        listItems.Add(item);

                        Console.WriteLine($"Id = {i}, PointOfSale = {item.PointOfSale}, Product = {item.Product}, Date = {item.Date}, Stock = {item.Stock}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in row {i}: {ex.Message}");
                        continue;
                    }
                }

                //save items in database 
                //if (listItems.Any())
                //{
                //    Console.WriteLine($"Reader list items: {listItems.Count()}");

                //    try
                //    {
                //        using (var context = new DBContext())
                //        {
                //            context.Items.AddRange(listItems);
                //            context.SaveChanges();
                //        }

                //        Console.WriteLine($"Saving list ({listItems.Count()}) items correct");
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine($"Error saving items, details: {ex.Message}");
                //    }
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error recorring fields to CSV.");
            }
        }

        public static void Logging()
        {
            var lastProcessedPoints = 0;
            while (!Exited)
            {
                Thread.Sleep(1000);
                var lapsed = (DateTime.Now - TestStart);
                var currentUsage = (GC.GetTotalMemory(false) / 1024 / 1024);
                var processedDelta = Math.Round((ProcessedPoints - lastProcessedPoints) * 2.0 / 1000);
                lastProcessedPoints = ProcessedPoints;

                Console.WriteLine("Time Elapsed {0}, Memory Used: {1}Mb, Aprox KPoints/Sec: {2}", lapsed, currentUsage, processedDelta);

                //if (currentUsage > StartingMb * 1.1)
                //{
                //    throw new Exception("Test1.Main(): TEST FAILED: EXCESS MEMORY USAGE");
                //}

                //if ((PointsCount / lapsed.TotalMilliseconds) < 100)
                //{
                //    throw new Exception("Test1.Main(): TEST FAILED: TOO SLOW");
                //}
            }
        }
    }
}
