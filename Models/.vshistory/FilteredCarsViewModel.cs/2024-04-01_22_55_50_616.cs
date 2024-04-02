namespace ImportExcelSql.Models
{
    public class CarFilter
    {
        public string Make { get; set; }
        public int? MinHorsePower { get; set; }
        public int? MaxHorsePower { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinDoors { get; set; }
        public int? MaxDoors { get; set; }
        public int? MinCylinders { get; set; }
        public int? MaxCylinders { get; set; }



    }

}
