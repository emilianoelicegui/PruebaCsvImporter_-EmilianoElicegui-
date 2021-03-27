using FileHelpers;
using System;
using System.Globalization;
using System.Linq;

namespace CsvImporter.Helpers
{
    [DelimitedRecord(";"), IgnoreFirst(1)]
    public class RecordClass
    {
        public string PointOfSale { get; set; }

        public string Product { get; set; }

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime Date { get; set; }

        [FieldConverter(ConverterKind.Int16)]
        public short Stock { get; set; }

    }
}
