using AutoMapper;
using CsvImporter.Profiles;
using CsvImporter.Repositories;
using CsvImporter.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Moq;
using System;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class CsvImporterTest
    {
        [TestMethod]
        public void TestServiceImporterCSV()
        {
            //ARRANGE            
            var moqConfiguration = new Mock<IConfiguration>();
            var moqRepositoryImporter = new Mock<IRepositoryImporter>();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ItemProfile());
            });

            var mapper = mockMapper.CreateMapper();

            IServiceImporter serviceImporter = new ServiceImporter(mapper, moqConfiguration.Object, moqRepositoryImporter.Object);

            //ACT
            serviceImporter.ImporterFileHelper("C:\\Users\\Emiliano Elicegui\\prueba.csv");
        }

        [TestMethod]
        public void TestRepositoryImporterCSV()
        {
            //ARRANGE            
            IRepositoryImporter repositoryImporter = new RepositoryImporter(new DBContext());

            var item = new Item()
            {
                PointOfSale = "ERES",
                Product = "3123F",
                Date = DateTime.Now,
                Stock = 2
            };

            List<Item> items = new List<Item>();
            items.Add(item);

            //ACT
            repositoryImporter.ImporterFileHelper(items);
        }
    }
}
