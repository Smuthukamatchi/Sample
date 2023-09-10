using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.Data;
using Sample.Models;
using System.Diagnostics;


namespace Sample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Home/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var employee = _dbContext.Employees.FirstOrDefault(employees => employees.Name == username && employees.Password == password);
            if (employee != null)
            {
            if (employee.Designation=="Admin")
            {
                _logger.LogInformation("Login by Admin");
                return RedirectToAction("AdminDashboard", "Admin");
            }
            else
            {
                if (employee != null)
                {
                    HttpContext.Session.SetString("EmployeeName", employee.Name);
                    _logger.LogInformation("Login by {UserName}", employee.Name);
                    return RedirectToAction("EmployeeDashboard", "Employee", new { name = employee.Name });
                }
            }
            }
            TempData["ErrorMessage"] = "Login failed. Please enter valid credentials.";
            return RedirectToAction("Login");
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
