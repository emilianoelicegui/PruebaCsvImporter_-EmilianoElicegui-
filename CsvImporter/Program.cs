using System;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CsvImporter.Services;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CsvImporter
{
    public class Program
    {
        public static bool Exited;
        public static DateTime TestStart;
        public static int ProcessedPoints;

        private readonly IConfiguration _configuration;
        private readonly IServiceImporter _serviceImporter;

        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Services.GetRequiredService<Program>().Run();
        }

        public Program(IConfiguration configuration, IHostingEnvironment env, IServiceImporter serviceImporter)
        {
            _configuration = configuration;
            _serviceImporter = serviceImporter;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appSettings.json")
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        public void Run()
        {
            var memoryLogger = new Thread(Logging); memoryLogger.Start();

            _serviceImporter.ImporterFileHelper();

            Exited = true;
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddTransient<Program>();
                    services.AddTransient<IServiceImporter, ServiceImporter>();

                    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                });
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
