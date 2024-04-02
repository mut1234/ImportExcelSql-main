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
    public class Import2Controller : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public Import2Controller(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public ActionResult File()
        {
            FileUploadViewModel model = new FileUploadViewModel();
            model.EmployeeListViewModel = new List<EmployeeViewModel>();
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
                    model.EmployeeListViewModel = new List<EmployeeViewModel>();
                    for (int row = 2; row <= rowCount; row++)
                    {
                        model.EmployeeListViewModel.Add(new EmployeeViewModel
                        {
                            FirstName = (worksheet.Cells[row, 2].Value ?? string.Empty).ToString().Trim(),
                            LastName = (worksheet.Cells[row, 5].Value ?? string.Empty).ToString().Trim(),
                            Email = (worksheet.Cells[row, 4].Value ?? string.Empty).ToString().Trim(),
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
            var list = new List<Student>();
            var totalStudentsSql = new List<Student>();

            foreach (var student in model.EmployeeListViewModel)
            {
                list.Add(new Student
                {
                    //StudentId = (int)(Double)worksheet.Cells[row, 1].Value,
                    Name = student.FirstName,
                    Adress = student.Email
                });
            }

            using (var context = new StudentDbContext())
            {
                context.Students.AddRange(list);
                context.SaveChanges();
                totalStudentsSql = context.Students.ToList();
            }
            //return same view and  pass view model 
            return View("View",totalStudentsSql);
        }

        [HttpPost]
        public async Task<List<Student>> Import(IFormFile file)
        {
            //IList<StudentSql> listStudents = new List<StudentSql>();
            var list = new List<Student>();
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
                        list.Add(new Student
                        {
                            //StudentId = (int)(Double)worksheet.Cells[row, 1].Value,
                            Name = worksheet.Cells[row, 2].Value.ToString().Trim(),
                            Adress = worksheet.Cells[row, 3].Value.ToString().Trim(),
                        });
                    }
                    using (var context = new StudentDbContext())
                    {
                        context.Students.AddRange(list);
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