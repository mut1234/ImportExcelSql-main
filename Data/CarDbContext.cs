using System;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;

#nullable disable

namespace ImportExcelSql.Models
{
    public partial class CarDbContext : DbContext
    {
        public CarDbContext()
        {
        }

        public CarDbContext(DbContextOptions<CarDbContext> options) : base(options)
        {
        }
        public DbSet<Car> Cars { get; set; }
        //---------------------------------

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }



    }
}

