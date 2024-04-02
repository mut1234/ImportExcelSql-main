using ImportExcelSql.Models;
using ImportExcelSql.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImportExcelSql.Controllers
{
    public class CarController : Controller
    {
        private readonly CarService _carService;

        public CarController(CarService carService)
        {
            _carService = carService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LoadCarsData(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                // Handle empty file error
                return BadRequest("Please upload a CSV file.");
            }

            // Call the CarService to load cars data
            var success = await _carService.LoadCarsDataAsync(csvFile);
            if (!success)
            {
                // Handle error while loading cars data
                return StatusCode(500, "Error occurred while loading cars data.");
            }

            return RedirectToAction("Index");
        }

        //[HttpPost]
        //public ActionResult FilterCars(CarFilter filter)
        //{
        //    // Call the CarService to filter cars
        //    var filteredCars = _carService.FilterCars(filter);

        //    // Pass filtered cars data to the view
        //    return View(filteredCars);
        //}
    }
}
