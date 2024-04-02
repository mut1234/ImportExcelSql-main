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

        [HttpPost]
        public async Task<ActionResult> File(FileUploadViewModel model)
        {
            if (model.CsvFile == null || model.CsvFile.Length == 0)
            {
                ModelState.AddModelError("CsvFile", "Please select a file.");
                return View(model);
            }

            try
            {
                // Load cars data from the uploaded CSV file using CarService
                bool result = await _carService.LoadCarsDataAsync(model.CsvFile);

                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Error occurred while processing the CSV file.");
                    return View(model);
                }

                // Filter the loaded cars data using CarService
                var filter = new CarFilter(); // You need to create and populate the filter based on user input
                var filteredCars = _carService.FilterCars(filter);

                // Map filtered cars data to view models
                model.CarListViewModel = filteredCars.Select(car => new CarViewModel
                {
                    carName = car.carName,
                    doorNumber = car.doorNumber,
                    bodyStyle = car.bodyStyle,
                    engineLocation = car.engineLocation,
                    numberOfCylinders = car.numberOfCylinders,
                    horsePower = car.horsePower,
                    price = car.price
                }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult FilterCars(CarFilter filter)
        {
            var filteredCars = _carService.FilterCars(filter);
            return View(filteredCars);
        }

    }
}