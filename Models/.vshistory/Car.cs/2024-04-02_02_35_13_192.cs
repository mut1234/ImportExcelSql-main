using System;
using System.Collections.Generic;

#nullable disable

namespace ImportExcelSql.Models
{
    public class Car
    {
        public int carId { get; set; }
        public string carName { get; set; }

        public string doorNumber { get; set; }

        public string bodyStyle { get; set; }

        public string engineLocation { get; set; }

        public string numberOfCylinders { get; set; }

        public int horsePower { get; set; }

        public int price { get; set; }
    }
}
