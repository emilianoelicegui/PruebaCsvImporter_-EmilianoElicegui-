using System;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CsvImporter.Services;
using Microsoft.Extensions.Configuration;
using Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using CsvImporter.Repositories;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CsvImporter
{
    public class Program
    {
        public static bool Exited;
        public static DateTime TestStart;
        public static int ProcessedPoints;

        private static IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceImporter _serviceImporter;

        public Program(IConfiguration configuration, ILoggerFactory loggerFactory, IServiceImporter serviceImporter)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
            _serviceImporter = serviceImporter;
        }

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("App Start");

            var host = CreateHostBuilder(args)
                .UseSerilog()
                .Build();

            host.Services.GetRequiredService<Program>().Run();
        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appSettings.json")
                   .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddTransient<Program>();
                    services.AddTransient<IServiceImporter, ServiceImporter>();
                    services.AddTransient<IRepositoryImporter, RepositoryImporter>();

                    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

                    AddEntityFramework(services);
                });
        }

        public static void AddEntityFramework(IServiceCollection services)
        {
            var connection = _configuration["Data:DefaultConnection"];
            
            services.AddDbContext<DBContext>(options => options.UseSqlServer(connection).ConfigureWarnings(warnings =>
            {
                warnings.Ignore(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning);
            })
            );
        }

        public void Run()
        {
            var memoryLogger = new Thread(Logging); memoryLogger.Start();

            _serviceImporter.ImporterFileHelper(_configuration["PathExcel"]);

            Log.Logger.Information("App stop");
            Exited = true;
        }

        public static void Logging()
        {
            Log.Logger.Information("Logging memory start");

            var lastProcessedPoints = 0;
            while (!Exited)
            {
                Thread.Sleep(30 * 1000);
                var lapsed = (DateTime.Now - TestStart);
                var currentUsage = (GC.GetTotalMemory(false) / 1024 / 1024);
                var processedDelta = Math.Round((ProcessedPoints - lastProcessedPoints) * 2.0 / 1000);
                lastProcessedPoints = ProcessedPoints;

                Log.Logger.Information("Time Elapsed {0}, Memory Used: {1}Mb, Aprox KPoints/Sec: {2}", lapsed, currentUsage, processedDelta);
            }
        }
    }
}
