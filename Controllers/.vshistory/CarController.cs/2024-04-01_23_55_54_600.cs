using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImportExcelSql.Models;
using ImportExcelSql.Service;
using Microsoft.AspNetCore.Hosting;

namespace ImportExcelSql.Controllers
{
    public class CarController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly CarService _carService;

        public CarController(IHostingEnvironment hostingEnvironment, CarService carService)
        {
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
            if (model.CsvFile == null || model.CsvFile.Length == 0)
            {
                ModelState.AddModelError("CsvFile", "Please select a file.");
                return View(model);
            }

            try
            {
                // Load cars data from the uploaded CSV file using CarService
                var result = await _carService.LoadCarsDataAsync(model.CsvFile);

                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Error occurred while processing the CSV file.");
                    return View(model);
                }

                // Reload the view with the updated model (empty model)
                model.CarListViewModel = new List<CarViewModel>();
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
