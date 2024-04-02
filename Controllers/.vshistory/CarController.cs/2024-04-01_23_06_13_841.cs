using Microsoft.AspNetCore.Mvc;
using ImportExcelSql.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ImportExcelSql.Service;

namespace ImportExcelSql.Controllers
{
    public class CarController : Controller
    {
        private readonly CarService _carService;

        public CarController(CarService carService)
        {
            _carService = carService;
        }

        // Action method to handle loading cars data and filtering
        [HttpPost]
        public async Task<ActionResult> LoadCarsDataAndFilter(FileUploadViewModel model)
        {
            // Call the LoadCarsDataAsync method of CarService to load cars data from CSV
            //bool success = await _carService.LoadCarsDataAsync(model.CsvFile);
            //if (!success)
            //{
            //    // Handle failure
            //    return BadRequest("Failed to load cars data.");
            //}

            // Call the FilterCars method of CarService to filter cars based on the provided filter
            CarFilter filter = new CarFilter(); // Create filter object based on your requirements
            var filteredCars = _carService.FilterCars(filter);

            // Pass the filtered cars data to the view
            return View(filteredCars);
        }
    }
}
