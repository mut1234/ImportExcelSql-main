﻿using Microsoft.AspNetCore.Mvc;
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
using ImportExcelSql.Data;
using Microsoft.EntityFrameworkCore;

namespace ImportExcelSql.Controllers
{
    public class CarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public CarController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
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
                    horsePower = int.TryParse(row[5].Trim(), out int hp) ? hp : 0, // Parse to integer, default to 0 if parsing fails
                    price = int.TryParse(row[6].Trim(), out int pr) ? pr : 0 // Parse to integer, default to 0 if parsing fails
                };

                model.CarListViewModel.Add(carViewModel);

                var carEntity = new Data.Car 
                {
                    CarName = carViewModel.carName,
                    DoorNumber = carViewModel.doorNumber,
                    BodyStyle = carViewModel.bodyStyle,
                    EngineLocation = carViewModel.engineLocation,
                    NumberOfCylinders = carViewModel.numberOfCylinders,
                    HorsePower = carViewModel.horsePower,
                    Price = carViewModel.price
                };

                _context.Cars.Add(carEntity);
            }

            // Return the view with the populated model
            return View(model);
        }

        [HttpPost]
        public ActionResult ImportOnly(FileUploadViewModel model)
        //public ActionResult ImportOnly(FormCollection form)
        {
            var list = new List<Data.Car>();
            var totalCarsSql = new List<Data.Car>();

            foreach (var car in model.CarListViewModel)
            {
                list.Add(new Data.Car
                {
                    //StudentId = (int)(Double)worksheet.Cells[row, 1].Value,
                    CarName = car.carName,
                    DoorNumber = car.doorNumber
                });
            }

            using (var context = new CarDbContext())
            {
                context.Cars.AddRange(list);
                context.SaveChanges();
                totalCarsSql = context.Cars.ToList();
            }
            //return same view and  pass view model 
            return View("View",totalCarsSql);
        }

        [HttpPost]
        public async Task<List<Data.Car>> Import(IFormFile file)
        {
            //IList<StudentSql> listStudents = new List<StudentSql>();
            var list = new List<Data.Car>();
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
                    using (var context = new CarDbContext())
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