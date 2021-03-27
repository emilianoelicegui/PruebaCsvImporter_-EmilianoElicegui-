using AutoMapper;
using CsvImporter.Helpers;
using Models;

namespace CsvImporter.Profiles
{
    public class ItemProfile : Profile 
    {
        public ItemProfile()
        {
            CreateMap<RecordClass, Item>();
        }
    }
}
