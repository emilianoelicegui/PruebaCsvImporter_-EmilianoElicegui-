using CsvImporter.Helpers.ObjectDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CsvImporter.Repositories
{
    public interface IRepositoryImporter
    {
        void ImporterFileHelper(IEnumerable<Item> items);
    }
    
    public class RepositoryImporter : IRepositoryImporter
    {
        private readonly DBContext _context;

        public RepositoryImporter(DBContext context)
        {
            _context = context;
        }

        public void ImporterFileHelper(IEnumerable<Item> items)
        {
            Log.Logger.Information("Run Importer File");

            try
            {
                var _connection = _context.Database.GetDbConnection();

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

                    Log.Logger.Information($"Saving list ({items.Count()}) items correct");
                }

                #endregion

                _context.Database.CommitTransaction();

            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                Log.Logger.Error(ex.Message);
            }
        }
    }
}
