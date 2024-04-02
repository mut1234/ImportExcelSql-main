using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

#nullable disable

namespace ImportExcelSql.Models
{
    public class Car
    {
        public IFormFile CsvFile { get; set; }
        public List<CarViewModel> CarListViewModel { get; set; }
        public StaffInfoViewModel StaffInfoViewModel { get; set; }
        //public List<CarViewModel> CarListViewModel { get; set; }
        public Car()//Create contractor
        {
            //call StaffInfoViewModel  this object in contractor
            StaffInfoViewModel = new StaffInfoViewModel();

        }
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
