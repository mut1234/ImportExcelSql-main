using System.Collections.Generic;

namespace ImportExcelSql.Models
{
    public class FilteredCarsViewModel
    {
        public string Make { get; set; }
        public int? MinHorsePower { get; set; }
        public int? MaxHorsePower { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public string NumDoors { get; set; }
        public string MinCylinders { get; set; }
        public string MaxCylinders { get; set; }
        public List<Car> FilteredCars { get; internal set; }


        public string bodyStyle { get; set; }

    }

}
