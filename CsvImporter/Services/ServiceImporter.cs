using AutoMapper;
using CsvImporter.Helpers;
using CsvImporter.Repositories;
using FileHelpers;
using Microsoft.Extensions.Configuration;
using Models;
using Serilog;
using System.Collections.Generic;

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

        private readonly IRepositoryImporter _repositoryImporter;

        public ServiceImporter(IMapper mapper, IConfiguration configuration, IRepositoryImporter repositoryImporter)
        {
            _mapper = mapper;
            _configuration = configuration;
            _repositoryImporter = repositoryImporter;
        }

        public void ImporterFileHelper()
        {
            Log.Logger.Information("Run Importer File");

            #region Read CSV 

            var engine = new FileHelperEngine<RecordClass>();
            var records = engine.ReadFile(_configuration["PathExcel"]);

            IEnumerable<Item> items = _mapper.Map<IEnumerable<Item>>(records);

            #endregion

            _repositoryImporter.ImporterFileHelper(items);
        }

    }
}
