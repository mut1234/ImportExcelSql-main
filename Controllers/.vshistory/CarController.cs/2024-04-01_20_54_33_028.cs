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

namespace ImportExcelSql.Controllers
{
    public class CarController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public CarController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
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

            // Map CSV data to CarViewModel
            model.CarListViewModel = new List<CarViewModel>();

            foreach (var row in csvData.Skip(1)) // Skip header row
            {
                var carViewModel = new CarViewModel
                {
                    carName = row[0].Trim(),
                    doorNumber = row[1].Trim(),
                    bodyStyle = row[2].Trim(),
                    engineLocation = row[3].Trim(),
                    numberOfCylinders = row[4].Trim(),
                    // Add mappings for other properties if needed
                };

                model.CarListViewModel.Add(carViewModel);
            }

            // Return the view with the populated model
            return View(model);
        }

        [HttpPost]
        public ActionResult ImportOnly(FileUploadViewModel model)
        //public ActionResult ImportOnly(FormCollection form)
        {
            var list = new List<Car>();
            var totalCarsSql = new List<Car>();

            foreach (var car in model.CarListViewModel)
            {
                list.Add(new Car
                {
                    //StudentId = (int)(Double)worksheet.Cells[row, 1].Value,
                    carName = car.carName,
                    doorNumber = car.doorNumber
                });
            }

            using (var context = new StudentDbContext())
            {
                context.Cars.AddRange(list);
                context.SaveChanges();
                totalCarsSql = context.Cars.ToList();
            }
            //return same view and  pass view model 
            return View("View",totalCarsSql);
        }

        [HttpPost]
        public async Task<List<Car>> Import(IFormFile file)
        {
            //IList<StudentSql> listStudents = new List<StudentSql>();
            var list = new List<Car>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowcount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowcount; row++)
                    {
                        list.Add(new Car
                        {
                            //StudentId = (int)(Double)worksheet.Cells[row, 1].Value,
                            carName = worksheet.Cells[row, 2].Value.ToString().Trim(),
                            doorNumber = worksheet.Cells[row, 3].Value.ToString().Trim(),
                        });
                    }
                    using (var context = new StudentDbContext())
                    {
                        context.Cars.AddRange(list);
                        context.SaveChanges();
                    }

                }

/*
                IList<StudentSql> newStudents = new List<StudentSql>() 
                        {
                                    new StudentSql() { StudentId = 6,  Name = "Steve",  Address = "Bill"}
//                                    new StudentSql() { Name = "Steve" }
//                                    new StudentSql() { Address = "Bill" }
//                                    new StudentSql() { Name = "Steve" },
                        };

                using (var context = new StudentDbContext())
                {
                    context.Students.AddRange(newStudents);
                    context.SaveChanges();
                }

*/
            }
            return list;
        }

    }
}