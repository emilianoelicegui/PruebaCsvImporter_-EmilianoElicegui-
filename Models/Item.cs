using System;

namespace Models
{
    public class Item
    {
        public int Id { get; set; }
        public string PointOfSale { get; set; }
        public string Product { get; set; }
        public DateTime Date { get; set; }
        public short Stock { get; set; }

    }
}
