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
using System.Data;
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
        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ServiceImporter(DBContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public void ImporterFileHelper()
        {
            try
            {
                var _connection = _context.Database.GetDbConnection();

                #region Read CSV 

                var engine = new FileHelperEngine<RecordClass>();
                var records = engine.ReadFile(_configuration.GetValue<string>("PathExcel"));

                List<Item> items = _mapper.Map<List<Item>>(records);

                #endregion

                #region Truncate table 

                using (var cmd = _connection.CreateCommand())
                {
                    if (_connection.State.Equals(ConnectionState.Closed))
                        _connection.Open();

                    cmd.CommandText = $"TRUNCATE TABLE Items";

                    cmd.ExecuteNonQuery();
                }

                if (_connection.State.Equals(ConnectionState.Open))
                    _connection.Close();

                #endregion

                _context.Database.BeginTransaction();

                #region Insert table 

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_connection.ConnectionString))
                {
                    bulkCopy.DestinationTableName = "dbo.Items";

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

                    Console.WriteLine($"Saving list ({items.Count()}) items correct");
                }

                #endregion

                _context.Database.CommitTransaction();

            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                Console.WriteLine($"Error saving items, details: {ex.Message}");
            }
        }

    }
}
