using ImportExcelSql.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Runtime.ConstrainedExecution;

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
        #region LoadCarsDataAsync
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
                            NumDoors = row[1].Trim(),
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
        
        public async Task<List<string[]>> ReadCsvDataAsync(IFormFile csvFile)
        {
            var csvData = new List<string[]>();

            using (var reader = new StreamReader(csvFile.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    string[] values = line.Split(','); // Split the line by comma
                    csvData.Add(values);
                }
            }

            return csvData;
        }

        public List<CarViewModel> MapCsvDataToCarViewModel(List<string[]> csvData)
        {
            var carListViewModel = new List<CarViewModel>();

            foreach (var row in csvData.Skip(1)) // Skip header row
            {
                var carViewModel = new CarViewModel
                {
                    carName = row[0].Trim(),
                    doorNumber = row[1].Trim(),
                    bodyStyle = row[2].Trim(),
                    engineLocation = row[3].Trim(),
                    numberOfCylinders = row[4].Trim(),
                    horsePower = int.TryParse(row[5].Trim(), out int hp) ? hp : 0, // Parse to integer, default to 0 if parsing fails
                    price = int.TryParse(row[6].Trim(), out int pr) ? pr : 0 // Parse to integer, default to 0 if parsing fails
                };

                carListViewModel.Add(carViewModel);
            }

            return carListViewModel;
        }
        #endregion

        #region FilterCars

        public List<Car> FilterCars(FilteredCarsViewModel filter)
        {
            // Apply filter criteria to the loaded cars data in the database
            var filteredCars = _context.Cars
                .Where(car =>
                    (filter.Make == null || car.carName.ToUpper() == filter.Make.ToUpper()) &&
                    (filter.bodyStyle == null || car.bodyStyle.ToUpper() == filter.bodyStyle.ToUpper()) &&
                    (filter.MinHorsePower == null || car.horsePower >= filter.MinHorsePower) &&
                    (filter.MaxHorsePower == null || car.horsePower <= filter.MaxHorsePower) &&
                    (filter.MinPrice == null || car.price >= filter.MinPrice)&&
                    (filter.MaxPrice == null || car.price <= filter.MaxPrice) &&
                    (filter.MinCylinders == null || car.numberOfCylinders.ToUpper() == filter.MinCylinders.ToUpper()) &&
                    (filter.MinCylinders == null || car.numberOfCylinders.ToUpper() == filter.MaxCylinders.ToUpper()) &&
                    (filter.NumDoors == null || car.NumDoors.ToUpper() == filter.NumDoors.ToUpper())
                    //    (filter.MinCylinders == null || ConvertToNumber(car.numberOfCylinders) >= filter.MinCylinders) &&
                    //    (filter.MaxCylinders == null || ConvertToNumber(car.numberOfCylinders) <= filter.MaxCylinders) &&
                    //    (ConvertToString(filter.NumDoors) == null || car.doorNumber.ToUpper() == ConvertToString(filter.NumDoors).ToUpper())
                ).ToList();

            return filteredCars;
        }
        //public int ConvertToNumber(string word)
        //{
        //    switch (word.ToLower())
        //    {
        //        case "one":
        //            return 1;
        //        case "two":
        //            return 2;
        //        case "three":
        //            return 3;
        //        case "four":
        //            return 4;
        //        case "five":
        //            return 5;
        //        case "six":
        //            return 6;
        //        default:
        //            return 0;
        //    }
        //}
        //public string ConvertToString(int? x)
        //{
        //    switch (x)
        //    {
        //        case 1:
        //            return "one";
        //        case 2:
        //            return "two";
        //        case 3:
        //            return "three";
        //        case 4:
        //            return "four";
        //        case 5:
        //            return "five";
        //        case 6:
        //            return "six";
        //        default:
        //            return "zero";
        //    }
        //}


        #endregion
    }
}
