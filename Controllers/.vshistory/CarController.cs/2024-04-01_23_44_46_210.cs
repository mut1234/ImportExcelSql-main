//using Microsoft.AspNetCore.Mvc;
//using OfficeOpenXml;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using ImportExcelSql.Models;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Hosting;
//using MySqlConnector;
//using Microsoft.EntityFrameworkCore;
//using ImportExcelSql.Service;
//using Microsoft.AspNetCore.Cors.Infrastructure;

//namespace ImportExcelSql.Controllers
//{
//    public class CarController : Controller
//    {
//        private readonly CarDbContext _context;
//        private readonly IHostingEnvironment _hostingEnvironment;
//        private readonly CarService _carService;

//        public CarController(CarDbContext context, IHostingEnvironment hostingEnvironment, CarService carService)
//        {
//            _context = context;
//            _hostingEnvironment = hostingEnvironment;
//            _carService = carService;

//        }


//        [HttpPost]
//        public async Task<ActionResult> LoadCarsData(IFormFile csvFile)
//        {
//            if (csvFile == null || csvFile.Length == 0)
//            {
//                // Handle empty file error
//                return BadRequest("Please upload a CSV file.");
//            }

//            // Call the CarService to load cars data
//            var success = await _carService.LoadCarsDataAsync(csvFile);
//            if (!success)
//            {
//                // Handle error while loading cars data
//                return StatusCode(500, "Error occurred while loading cars data.");
//            }

//            return RedirectToAction("Index");
//        }


//        [HttpPost]
//        public ActionResult FilterCars(CarFilter filter)
//        {
//            var filteredCars = _carService.FilterCars(filter);
//            return View(filteredCars);
//        }

//    }
//}