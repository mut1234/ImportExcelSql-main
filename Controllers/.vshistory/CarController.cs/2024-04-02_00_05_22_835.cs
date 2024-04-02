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
            // Check if a file is provided
            if (model.CsvFile == null || model.CsvFile.Length == 0)
            {
                ModelState.AddModelError("CsvFile", "Please select a file.");
                return View(model);
            }

            // Call the service to load cars data from the CSV file
            bool success = await _carService.LoadCarsDataAsync(model.CsvFile);

            if (!success)
            {
                ModelState.AddModelError("", "Error occurred while processing the CSV file.");
                return View(model);
            }

            // Redirect to the same view to display the uploaded cars data
            return RedirectToAction("File");
        }


        [HttpPost]
        public ActionResult FilterCars(CarFilter filter)
        {
            var filteredCars = _carService.FilterCars(filter);
            return View(filteredCars);
        }

    }
}