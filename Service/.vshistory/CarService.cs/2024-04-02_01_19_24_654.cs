using ImportExcelSql.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;

namespace ImportExcelSql.Service
{
    public class CarService
    {
        private readonly CarDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public CarService(CarDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        /*
        Loads cars data from "autos.csv" file and store in memory. 
        If cars data is already loaded, then this method should not do anything.
        */
        public async Task<bool> LoadCarsDataAsync(IFormFile csvFile)
        {
            try
            {
                // Read the content of the CSV file
                using (var stream = csvFile.OpenReadStream())
                using (var reader = new StreamReader(stream))
                {
                    var csvData = new List<string[]>();

                    while (!reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();
                        string[] values = line.Split(','); // Split the line by comma
                        csvData.Add(values);
                    }

                    // Save cars data to the database
                    foreach (var row in csvData.Skip(1)) // Skip header row
                    {
                        var carEntity = new Car
                        {
                            carName = row[0].Trim(),
                            doorNumber = row[1].Trim(),
                            bodyStyle = row[2].Trim(),
                            engineLocation = row[3].Trim(),
                            numberOfCylinders = row[4].Trim(),
                            horsePower = int.TryParse(row[5].Trim(), out int hp) ? hp : 0, // Parse to integer, default to 0 if parsing fails
                            price = int.TryParse(row[6].Trim(), out int pr) ? pr : 0 // Parse to integer, default to 0 if parsing fails
                        };

                        _context.Cars.Add(carEntity);
                    }

                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error occurred while processing the CSV file: {ex.Message}");
                return false;
            }
        }
        private int ConvertToNumber(string word)
        {
            switch (word.ToLower())
            {
                case "one":
                    return 1;
                case "two":
                    return 2;
                case "three":
                    return 3;
                case "four":
                    return 4;
                case "five":
                    return 5;
                case "six":
                    return 6;
                default:
                    return 0; 
            }
        }

        /*
        Filters the loaded cars data using the provided filter and returns a list of cars. 
        The caller of this method should be able to filter cars based on their make, horsepower range, price range, number of doors, and number of cylinders.
        */
        public List<Car> FilterCars(FilteredCarsViewModel filter)
        {
            // Apply filter criteria to the loaded cars data in the database
            var filteredCars = _context.Cars
         .Where(car =>
         (filter.Make == null || car.carName.ToUpper() == filter.Make.ToUpper()) &&
         (filter.MinHorsePower == null || car.horsePower >= filter.MinHorsePower) &&
         (filter.MaxHorsePower == null || car.horsePower <= filter.MaxHorsePower) &&
         (filter.MinPrice == null || car.price >= filter.MinPrice) &&
         (filter.MaxPrice == null || car.price <= filter.MaxPrice) &&
         (filter.MinCylinders == null || ConvertToNumber(car.numberOfCylinders) >= filter.MinCylinders) &&
         (filter.MaxCylinders == null || ConvertToNumber(car.numberOfCylinders) <= filter.MaxCylinders) &&
         (filter.MinDoors == null || ConvertToNumber(car.doorNumber) >= filter.MinDoors) &&
         (filter.MaxDoors == null || ConvertToNumber(car.doorNumber) <= filter.MaxDoors))
        .ToList();


            return filteredCars;
        }
    }

}
