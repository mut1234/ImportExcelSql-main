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
            if (model.CsvFile != null && model.CsvFile.Length > 0)
            {
                // Call the service to load cars data
                bool success = await _carService.LoadCarsDataAsync(model.CsvFile);

                if (success)
                {
                    // If data loaded successfully, populate the CarListViewModel and return the view
                    List<string[]> csvData = await _carService.ReadCsvDataAsync(model.CsvFile);
                    model.CarListViewModel = _carService.MapCsvDataToCarViewModel(csvData);

                    return View(model);
                }
                else
                {
                    // If an error occurred while loading data, return the same view with the model
                    ModelState.AddModelError(string.Empty, "An error occurred while processing the CSV file.");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please select a CSV file to upload.");
                return View(model);
            }
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