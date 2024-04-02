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
        public ActionResult File(FileUploadViewModel model)
        {
            string rootFolder = _hostingEnvironment.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + model.XlsFile.FileName;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
            using (var stream = new MemoryStream())
            {
                model.XlsFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    package.SaveAs(file);
                    //save excel file in your wwwroot folder and get this excel file from wwwroot
                }
            }
            //After save excel file in wwwroot and then
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    //return or alert message here
                }
                else
                {
                    //read excel file data and add data in  model.StaffInfoViewModel.StaffList
                    var rowCount = worksheet.Dimension.Rows;
                    model.CarListViewModel = new List<CarViewModel>();
                    for (int row = 2; row <= rowCount; row++)
                    {
                        model.CarListViewModel.Add(new CarViewModel
                        {
                            carName = (worksheet.Cells[row, 1].Value ?? string.Empty).ToString().Trim(),
                            doorNumber = (worksheet.Cells[row, 2].Value ?? string.Empty).ToString().Trim(),
                            bodyStyle = (worksheet.Cells[row, 3].Value ?? string.Empty).ToString().Trim(),
                            engineLocation = (worksheet.Cells[row, 4].Value ?? string.Empty).ToString().Trim(),
                            numberOfCylinders = (worksheet.Cells[row, 5].Value ?? string.Empty).ToString().Trim(),
                           // horsePower = ((int)worksheet.Cells[row, 6].Value),
                           //  price = ((int)worksheet.Cells[row, 7].Value),
                        });
                    }
                }
            }
            //return same view and  pass view model 
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