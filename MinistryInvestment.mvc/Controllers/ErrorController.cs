using Microsoft.AspNetCore.Mvc;
using MinistryInvestment.Mvc.ViewModels;
using System.Diagnostics;

namespace MinistryInvestment.Mvc.Controllers
{
    public class ErrorController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            return View(new ErrorViewModel(Activity.Current?.Id ?? HttpContext.TraceIdentifier));
        }
    }
}