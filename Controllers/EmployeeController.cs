using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sample.Data;
using Sample.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace Sample.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public EmployeeController(ILogger<EmployeeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

    
        public IActionResult EmployeeDashboard(string name)
        {
            ViewData["EmployeeName"] = name;
            return View("~/Views/Employee/EmployeeDashboard.cshtml");
        }

        [HttpGet]
        public IActionResult ViewProfile()
        {
            string employeeName = HttpContext.Session.GetString("EmployeeName");

            var employee = _dbContext.Employees.FirstOrDefault(employees => employees.Name == employeeName);

            if (employee != null)
            {
                _logger.LogInformation("Profile viewed by {UserName}", employee.Name);
                ViewData["EmployeeName"] = employee.Name;
                ViewData["EmployeeId"] = employee.EmpId;
                ViewData["EmployeeEmail"] = employee.Email;
                ViewData["EmployeePhone"] = employee.ContactNumber;
                ViewData["EmployeeLocation"] = employee.Location;
                ViewData["EmployeeDesignation"] = employee.Designation;
                ViewData["EmployeeDOB"] = employee.DateOfBirth.ToShortDateString();
                ViewData["EmployeeDOJ"] = employee.DateOfJoin.ToShortDateString();
                ViewData["EmployeePassword"] = employee.Password;

                return View("~/Views/Employee/ViewProfile.cshtml");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpGet]
        public IActionResult AddAttendance()
        {
            string? employeeName = HttpContext.Session.GetString("EmployeeName");

            var employee = _dbContext.Employees.FirstOrDefault(employees => employees.Name == employeeName);

            if (employee != null)
            {
                ViewBag.EmployeeName = employee.Name;
                HttpContext.Session.SetString("EmployeeName", employee.Name);
                


                var attendance = new Attendance
                {
                    EmpId = employee.EmpId,
                    EmployeeName = employee.Name,
                    AttendanceDate = DateTime.Now
                };

                return View("~/Views/Employee/AddAttendance.cshtml", attendance);
            }
            else
            {
                return RedirectToAction("EmployeeLogin");
            }
        }

        [HttpPost]
        public IActionResult AddAttendance(Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                string? employeeName = HttpContext.Session.GetString("EmployeeName");
                var employee = _dbContext.Employees.FirstOrDefault(employees => employees.Name == employeeName);
                _logger.LogInformation("Attendance added by {UserName}", employeeName);
                
                if (employee != null)
                {
                    attendance.EmpId = employee.EmpId;
                    attendance.EmployeeName = employee.Name;
                    attendance.AttendanceDate = DateTime.Now;

                    _dbContext.Attendances.Add(attendance);
                    _dbContext.SaveChanges();

                    TempData["SuccessMessage"] = "Attendance added successfully.";
                }
                else
                {
                    return RedirectToAction("EmployeeLogin");
                }

                return RedirectToAction("AddAttendance");
            }

            return View(attendance);
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            string? employeeName = HttpContext.Session.GetString("EmployeeName");

            var employee = _dbContext.Employees.FirstOrDefault(employees => employees.Name == employeeName);

            if (employee != null)
            {
                // _logger.LogInformation("Profile Updated by {UserName}", employee.Name);
                return View("~/Views/Employee/EditProfile.cshtml", employee);
            }

            TempData["ErrorMessage"] = "Employee not found.";
            return RedirectToAction("EmployeeDashboard");
        }

        [HttpPost]
        public IActionResult UpdateProfile(Employee updatedEmployee)
        {
            if (ModelState.IsValid)
            {
                var employee = _dbContext.Employees.FirstOrDefault(employees => employees.EmpId == updatedEmployee.EmpId);
                _logger.LogInformation("Profile Updated by {UserName}", updatedEmployee.Name);


                if (employee != null)
                {
                    employee.Name = updatedEmployee.Name;
                    employee.Email = updatedEmployee.Email;
                    employee.ContactNumber = updatedEmployee.ContactNumber;
                    employee.Location = updatedEmployee.Location;
                    employee.Designation = updatedEmployee.Designation;
                    employee.DateOfBirth = updatedEmployee.DateOfBirth;
                    employee.DateOfJoin = updatedEmployee.DateOfJoin;
                    employee.Password = updatedEmployee.Password; 
                    _dbContext.SaveChanges();

                    TempData["SuccessMessage"] = "Profile updated successfully.";
                    return RedirectToAction("ViewProfile", "Employee");
                }
            }

            TempData["SuccessMessage"] = "Failed to update profile.";
            return RedirectToAction("ViewProfile", "Employee");
        }


        [HttpGet]
        public IActionResult ViewSalary()
        {
            string? employeeName = HttpContext.Session.GetString("EmployeeName");

            var salary = _dbContext.Salaries
                .Where(employees => employees.EmployeeName == employeeName)
                .OrderByDescending(employees => employees.SalaryDate)
                .FirstOrDefault();

            if (salary != null)
            {
                ViewData["EmployeeName"] = salary.EmployeeName;
                ViewData["EmployeeId"] = salary.EmpId;
                ViewData["EmployeeDesignation"] = salary.Designation;
                ViewData["SalaryDate"] = salary.SalaryDate.ToShortDateString();
                ViewData["SalaryAmount"] = salary.SalaryAmount;
                ViewData["TaxAmount"] = salary.TaxAmount;
                ViewData["TotalPFAmount"] = salary.TotalPFAmount;
                ViewData["NetSalary"] = salary.NetSalary;

                var selectedYear = salary.SalaryDate.Year;
                var totalSalaryForSelectedYear = _dbContext.Salaries
                    .Where(employees => employees.EmployeeName == employeeName && employees.SalaryDate.Year == selectedYear)
                    .Sum(employees => employees.SalaryAmount);
                ViewData["TotalSalaryForSelectedYear"] = totalSalaryForSelectedYear;

                return View("~/Views/Employee/ViewSalary.cshtml");
            }
            else
            {
                return RedirectToAction("ViewSalary");
            }
        }

        [HttpPost]
        public IActionResult ViewSalary(DateTime selectedDate)
        {
            string? employeeName = HttpContext.Session.GetString("EmployeeName");
            _logger.LogInformation("Salary Viewed by {UserName}", employeeName);
            int selectedYear = selectedDate.Year;

            var salary = _dbContext.Salaries
                .Where(employees => employees.EmployeeName == employeeName && employees.SalaryDate.Month == selectedDate.Month && employees.SalaryDate.Year == selectedDate.Year)
                .FirstOrDefault();

            if (salary != null)
            {
                ViewData["EmployeeName"] = salary.EmployeeName;
                ViewData["EmployeeId"] = salary.EmpId;
                ViewData["EmployeeDesignation"] = salary.Designation;
                ViewData["SalaryDate"] = salary.SalaryDate.ToShortDateString();
                ViewData["SalaryAmount"] = salary.SalaryAmount;
                ViewData["TaxAmount"] = salary.TaxAmount;
                ViewData["TotalPFAmount"] = salary.TotalPFAmount;
                ViewData["NetSalary"] = salary.NetSalary;

                decimal totalSalaryForSelectedYear = _dbContext.Salaries
                    .Where(e => e.EmployeeName == employeeName && e.SalaryDate.Year == selectedYear)
                    .Sum(e => e.SalaryAmount);

                ViewData["TotalSalaryForSelectedYear"] = totalSalaryForSelectedYear;

                return View("~/Views/Employee/ViewSalary.cshtml");
            }
            else
            {
                return RedirectToAction("ViewSalary");
            }
        }
    }
}
