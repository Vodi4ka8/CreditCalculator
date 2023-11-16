using CreditCalculator.Models;
using Microsoft.AspNetCore.Mvc;

namespace CreditCalculator.Controllers
{
    public class ResultsController : Controller
    {
        // GET: ResultsController
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Index(List<PaymentResultModel> results)
        {
            return View();
        }

    }
}
