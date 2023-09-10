using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sample.Data;
using Sample.Models;
using System.Collections.Generic;

namespace Sample.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public AdminController(ILogger<AdminController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

       
        public IActionResult AdminDashboard(string name)
        {
            return View("~/Views/Admin/AdminDashboard.cshtml");
        }

        [HttpGet]
        public IActionResult ViewEmployee(string searchDesignation)
        {
            if (string.IsNullOrEmpty(searchDesignation))
            {
                List<Employee> employees = _dbContext.Employees.ToList();

                employees = employees.Where(employee => !string.Equals(employee.Designation, "Admin", StringComparison.OrdinalIgnoreCase)).ToList();

                return View("~/Views/Admin/ViewEmployee.cshtml", employees);
            }
            else
            {
                List<Employee> employees = _dbContext.Employees
                    .AsEnumerable()
                    .Where(employee => !string.IsNullOrEmpty(employee.Designation) && 
                                employee.Designation.Contains(searchDesignation, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                employees = employees.Where(employee => !string.Equals(employee.Designation, "Admin", StringComparison.OrdinalIgnoreCase)).ToList();

                if (employees.Count == 0)
                {
                    TempData["SearchError"] = "No employees found with the entered designation.";
                }

                return View("~/Views/Admin/ViewEmployee.cshtml", employees);
            }
        }



        [HttpGet]
        public IActionResult ViewEmployeeDetails(int id)
        {
            var employee = _dbContext.Employees.FirstOrDefault(employees => employees.EmpId == id);

            if (employee != null)
            {
                return View("~/Views/Admin/ViewEmployeeDetails.cshtml", employee);
            }
            else
            {
                return RedirectToAction("ViewEmployee");
            }
        }

        [HttpGet]
        public IActionResult ViewAttendance()
        {
            var attendances = _dbContext.Attendances.ToList();
            return View("~/Views/Admin/ViewAttendance.cshtml", attendances);
        }

        [HttpGet]
        public IActionResult AttendanceDetails(int empId)
        {
            var employee = _dbContext.Employees.FirstOrDefault(employees => employees.EmpId == empId);

            if (employee == null)
            {
                return NotFound();
            }

            var attendances = _dbContext.Attendances
                .Where(attendance => attendance.EmpId == empId)
                .ToList();

            ViewData["AttendanceCount"] = 0;
            ViewData["EmployeeName"] = employee.Name;

            return View("~/Views/Admin/AttendanceDetails.cshtml", attendances);
        }
        
        [HttpGet]
        public IActionResult AddEmployee()
        {
            return View("~/Views/Admin/AddEmployee.cshtml");
        }

        [HttpPost]
        public IActionResult AddEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                 _logger.LogInformation("Employee Added by Admin");

                _dbContext.Employees.Add(employee);
                _dbContext.SaveChanges();

                TempData["SuccessMessage"] = "Employee added successfully.";

                return RedirectToAction("AddEmployee", "Admin");
            }

            return View("~/Views/Admin/AddEmployee.cshtml", employee);
        }

        [HttpGet]
        public IActionResult Salary()
        {
            List<Employee> employees = _dbContext.Employees.ToList();

            employees = employees
                .Where(employee => !string.Equals(employee.Designation, "Admin", StringComparison.OrdinalIgnoreCase))
                .ToList();

            return View("~/Views/Admin/Salary.cshtml", employees);
        }


       [HttpGet]
        public IActionResult AddSalary(int id)
        {
            var employee = _dbContext.Employees.FirstOrDefault(employees => employees.EmpId == id);

            if (employee != null)
            {
                DateTime currentDate = DateTime.Now;
                DateTime startDate = new DateTime(currentDate.Year, currentDate.Month - 1, 1);
                DateTime endDate = new DateTime(currentDate.Year, currentDate.Month, 1).AddDays(-1);

                int attendanceCount = _dbContext.Attendances
                    .Where(attendance => attendance.EmpId == id && attendance.AttendanceDate >= startDate && attendance.AttendanceDate <= endDate)
                    .Count();
                ViewData["AttendanceCount"] = attendanceCount;
                return View("~/Views/Admin/AddSalary.cshtml", employee);
            }
            else
            {
                return RedirectToAction("AddSalary", "Home");
            }
        }

        private string GetEmployeeDesignation(int empId) // Helper method
        {
            var employee = _dbContext.Employees.FirstOrDefault(employees => employees.EmpId == empId);
            if (employee != null)
            {
                return employee.Designation;
            }
            return string.Empty;
        }

        [HttpPost]
        public IActionResult AddSalary(int empId, decimal salary, int workingDays)
        {
            var employee = _dbContext.Employees.FirstOrDefault(employees => employees.EmpId == empId);
            _logger.LogInformation("Salary Added to {ID}", empId);
            if (employee != null)
            {
                employee.Designation = GetEmployeeDesignation(empId);
                // Tax Calculation
                decimal taxAmount = 0;

                if (salary <= 25000)
                {
                    taxAmount = 0;
                }
                else if (salary <= 50000)
                {
                    taxAmount = (salary - 25000) * 0.05m;
                }
                else if (salary <= 75000)
                {
                    taxAmount = 1250 + (salary - 50000) * 0.1m;
                }

                // PF Calculation
                decimal EmpContributionPercentage = 12.00m; // Employee contribution percentage is 12%
                decimal EmployerEPFPercentage = 3.33m; // Employer EPF  percentage is 3.33%
                decimal EmployerEPSPercentage = 3.67m; // Employer EPS  percentage is 3.67%

                // Calculate the total PF amount
                decimal totalPFAmount = (EmpContributionPercentage / 100) * salary +
                                        (EmployerEPFPercentage / 100) * salary +
                                        (EmployerEPSPercentage / 100) * salary;

                decimal netSalary = salary - taxAmount - totalPFAmount;

                DateTime currentDate = DateTime.Now;
                DateTime startDate = new DateTime(currentDate.Year, currentDate.Month - 1, 1);
                DateTime endDate = new DateTime(currentDate.Year, currentDate.Month, 1).AddDays(-1);

                int attendanceCount = _dbContext.Attendances
                    .Where(attendance => attendance.EmpId == empId && attendance.AttendanceDate >= startDate && attendance.AttendanceDate <= endDate)
                    .Count();

                decimal salaryPerDay = Math.Round(salary / workingDays, 2);
                decimal deduction = 0;
                decimal leaveDays = 0;

                if (attendanceCount < workingDays - 2 && attendanceCount != 0)
                {
                    leaveDays = ((workingDays - attendanceCount) - 2);
                    deduction = Math.Round(salaryPerDay * leaveDays, 2);

                    // Subtract the deduction from the net salary
                    netSalary -= deduction;
                }


                // Console.WriteLine($"Attendance Count: {attendanceCount}");
                // Console.WriteLine($"Leavedays: {leaveDays}");
                // Console.WriteLine($"Salary Per Day: {salaryPerDay}");
                // Console.WriteLine($"Deduction: {deduction}");
                // Console.WriteLine($"Net Salary: {netSalary}");

                var newSalary = new Salary(empId)
                {
                    EmployeeName = employee.Name,
                    Designation = employee.Designation,
                    SalaryDate = DateTime.Now,
                    SalaryAmount = salary,
                    TaxAmount = taxAmount,
                    TotalPFAmount = totalPFAmount,
                    NetSalary = netSalary
                };

                _dbContext.Salaries.Add(newSalary);
                _dbContext.SaveChanges();

                TempData["SuccessMessage"] = "Salary added successfully.";

                return RedirectToAction("Salary");
            }

            return RedirectToAction("Salary");
        }

    }

}
