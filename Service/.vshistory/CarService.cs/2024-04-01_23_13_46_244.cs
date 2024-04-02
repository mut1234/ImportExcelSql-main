//using ImportExcelSql.Models;
//using Microsoft.AspNetCore.Http;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading.Tasks;
//using System;
//using Microsoft.AspNetCore.Hosting;
//using System.Linq;

//namespace ImportExcelSql.Service
//{
//    public class CarService
//    {
//        private readonly CarDbContext _context;
//        private readonly IHostingEnvironment _hostingEnvironment;

//        public CarService(CarDbContext context, IHostingEnvironment hostingEnvironment)
//        {
//            _context = context;
//            _hostingEnvironment = hostingEnvironment;
//        }

//        /*
//        Loads cars data from "autos.csv" file and store in memory. 
//        If cars data is already loaded, then this method should not do anything.
//        */
//        public async Task<bool> LoadCarsDataAsync(IFormFile csvFile)
//        {
//            string filePath = null;

//            try
//            {
//                // Load Cars Data
//                string rootFolder = _hostingEnvironment.WebRootPath;
//                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(csvFile.FileName);
//                filePath = Path.Combine(rootFolder, fileName);

//                // Save the uploaded CSV file
//                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
//                {
//                    await csvFile.CopyToAsync(fileStream);
//                }

//                // Read the content of the CSV file
//                List<string[]> csvData = new List<string[]>();

//                using (StreamReader reader = new StreamReader(filePath))
//                {
//                    while (!reader.EndOfStream)
//                    {
//                        string line = await reader.ReadLineAsync();
//                        string[] values = line.Split(','); // Split the line by comma
//                        csvData.Add(values);
//                    }
//                }

//                // Save cars data to the database
//                foreach (var row in csvData.Skip(1)) // Skip header row
//                {
//                    var carEntity = new Car
//                    {
//                        carName = row[0].Trim(),
//                        doorNumber = row[1].Trim(),
//                        bodyStyle = row[2].Trim(),
//                        engineLocation = row[3].Trim(),
//                        numberOfCylinders = row[4].Trim(),
//                        horsePower = int.TryParse(row[5].Trim(), out int hp) ? hp : 0, // Parse to integer, default to 0 if parsing fails
//                        price = int.TryParse(row[6].Trim(), out int pr) ? pr : 0 // Parse to integer, default to 0 if parsing fails
//                    };

//                    _context.Cars.Add(carEntity);
//                }

//                await _context.SaveChangesAsync();

//                return true;
//            }
//            catch (Exception ex)
//            {
//                // Handle exception
//                Console.WriteLine("Error occurred while processing the CSV file: " + ex.Message);
//                return false;
//            }
//            finally
//            {
//                // Delete the temporary file
//                if (filePath != null && File.Exists(filePath))
//                {
//                    File.Delete(filePath);
//                }
//            }
//        }

//        /*
//        Filters the loaded cars data using the provided filter and returns a list of cars. 
//        The caller of this method should be able to filter cars based on their make, horsepower range, price range, number of doors, and number of cylinders.
//        */
//        public List<Car> FilterCars(CarFilter filter)
//        {
//            // Apply filter criteria to the loaded cars data in the database
//            var filteredCars = _context.Cars
//                .Where(car =>
//                    (filter.Make == null || car.carName.Equals(filter.Make, StringComparison.OrdinalIgnoreCase)) &&
//                    (filter.MinHorsePower == null || car.horsePower >= filter.MinHorsePower) &&
//                    (filter.MaxHorsePower == null || car.horsePower <= filter.MaxHorsePower) &&
//                    (filter.MinPrice == null || car.price >= filter.MinPrice) &&
//                    (filter.MaxPrice == null || car.price <= filter.MaxPrice) //&&
//                //    (filter.MinDoors == null || int.TryParse(car.doorNumber, out int doors) && doors >= filter.MinDoors) &&
//                //(filter.MaxDoors == null || int.TryParse(car.doorNumber, out doors) && doors <= filter.MaxDoors) &&
//                //(filter.MinCylinders == null || int.TryParse(car.numberOfCylinders, out int cylinders) && cylinders >= filter.MinCylinders) &&
//                //(filter.MaxCylinders == null || int.TryParse(car.numberOfCylinders, out cylinders) && cylinders <= filter.MaxCylinders)

//                ).ToList();

//            return filteredCars;
//        }
//    }

//}
