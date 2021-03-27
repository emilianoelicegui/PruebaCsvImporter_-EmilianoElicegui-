using AutoMapper;
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
            
            foreach (var i in items)
            {
                Console.WriteLine(i.PointOfSale);
                Console.WriteLine(i.Product);
                Console.WriteLine(i.Date.ToString("dd/MM/yyyy"));
                Console.WriteLine(i.Stock.ToString());
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
