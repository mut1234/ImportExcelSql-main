using Microsoft.EntityFrameworkCore;

namespace ImportExcelSql.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
    }

    public class Car
    {
        public int Id { get; set; }
        public string CarName { get; set; }
        public string DoorNumber { get; set; }
        public string BodyStyle { get; set; }
        public string EngineLocation { get; set; }
        public string NumberOfCylinders { get; set; }
        public int HorsePower { get; set; }
        public int Price { get; set; }
    }

}
