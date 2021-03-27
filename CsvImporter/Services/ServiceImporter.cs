using AutoMapper;
using CsvImporter.Helpers;
using CsvImporter.Helpers.ObjectDataReader;
using FileHelpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvImporter.Services
{
    public interface IServiceImporter
    {
        void ImporterFileHelper();
    }

    public class ServiceImporter : IServiceImporter
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ServiceImporter(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
        }

        public void ImporterFileHelper()
        {
            var engine = new FileHelperEngine<RecordClass>();
            var records = engine.ReadFile(_configuration.GetValue<string>("PathExcel"));

            List<Item> items = _mapper.Map<List<Item>>(records);

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
        }

    }
}
