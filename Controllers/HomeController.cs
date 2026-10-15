using Microsoft.AspNetCore.Mvc;

namespace CollegeManagementApp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Error() => View();
}
