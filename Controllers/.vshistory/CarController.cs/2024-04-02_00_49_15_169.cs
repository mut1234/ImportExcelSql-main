using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using ImportExcelSql.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using MySqlConnector;
using Microsoft.EntityFrameworkCore;
using ImportExcelSql.Service;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace ImportExcelSql.Controllers
{
    public class CarController : Controller
    {
        private readonly CarDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly CarService _carService;

        public CarController(CarDbContext context, IHostingEnvironment hostingEnvironment, CarService carService)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _carService = carService;

        }

        public ActionResult File()
        {
            FileUploadViewModel model = new FileUploadViewModel();
            model.CarListViewModel = new List<CarViewModel>();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> File(FileUploadViewModel model)
        {
            //LoadCars Data
            string rootFolder = _hostingEnvironment.WebRootPath;
        
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.CsvFile.FileName);
            string filePath = Path.Combine(rootFolder, fileName);

            // Save the uploaded CSV file
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.CsvFile.CopyToAsync(fileStream);
            }

            // Read the content of the CSV file
            List<string[]> csvData = new List<string[]>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    string[] values = line.Split(','); // Split the line by comma
                    csvData.Add(values);
                }
            }

            model.CarListViewModel = new List<CarViewModel>();
            //Car Filter
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

                model.CarListViewModel.Add(carViewModel);

                var carEntity = new Car 
                {
                    carName = carViewModel.carName,
                    doorNumber = carViewModel.doorNumber,
                    bodyStyle = carViewModel.bodyStyle,
                    engineLocation = carViewModel.engineLocation,
                    numberOfCylinders = carViewModel.numberOfCylinders,
                    horsePower = carViewModel.horsePower,
                    price = carViewModel.price
                };

                _context.Cars.Add(carEntity);
               await _context.SaveChangesAsync();
            }

            return View(model);
        }

        [HttpGet("CarFilters")]
        public ActionResult FilterCars(FilteredCarsViewModel filter)
        {
            var filteredCars = _carService.FilterCars(filter);

            // Create an instance of FilteredCarsViewModel and set its properties
            var viewModel = new FilteredCarsViewModel
            {
                FilteredCars = filteredCars
            };

            // Pass the viewModel to the view
            return View(viewModel);
        }


    }
}