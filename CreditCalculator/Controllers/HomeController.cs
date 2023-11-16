using CreditCalculator.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CreditCalculator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult ViewForPerMonth()
        {
            return View("PerMonth");
        }
        public IActionResult ViewForPerDay()
        {
            return View("PerDay");
        }

        [HttpPost]
        public IActionResult IndexPerMonth(CreditInformationModel creditInformation)
        {
            if (ModelState.IsValid)
            {
                decimal balance = creditInformation.Sum;
                decimal totalOverpaid = 0;
                List<PaymentResultModel> results = new List<PaymentResultModel>();
                for (int j = 0; j < creditInformation.Time; j++)
                {
                    PaymentResultModel result = CreateResultModel(creditInformation, j, ref balance);
                    totalOverpaid += result.Percent;
                    results.Add(result);
                }
                TempData["totalOverpaid"] = totalOverpaid;
                return View("Results", results);


            }
            return View(creditInformation);
        }

        [HttpPost]
        public IActionResult IndexPerDay(DayCreditInformationModel creditInformation)
        {
            if (ModelState.IsValid)
            {
                decimal balance = creditInformation.Sum;
                decimal totalOverpaid = 0;
                List<PaymentResultModel> results = new List<PaymentResultModel>();
                for (int j = 0; j < creditInformation.Time; j++)
                {
                    PaymentResultModel result = DayCreateResultModel(creditInformation, j, ref balance);
                    totalOverpaid += result.Percent;
                    results.Add(result);
                }
                TempData["totalOverpaid"] = totalOverpaid;
                return View("Results", results);
            }
            return View(creditInformation);
        }

        private PaymentResultModel CreateResultModel(CreditInformationModel creditInformation, int index, ref decimal balance)
        {

            PaymentResultModel result = new PaymentResultModel();

            result.ID = index + 1;
            decimal i = creditInformation.Rate / 12 / 100; // /100 is to deal with percents. i - monthly interest rate
            decimal power = (decimal)Math.Pow((double)(1 + i), creditInformation.Time);
            decimal сoef = (i * power) / (power - 1);
            decimal total = сoef * creditInformation.Sum;
            decimal percent = balance / 12 / 100 * creditInformation.Rate;
            decimal part = total - percent;
            balance -= part;
            result.Balance = balance;
            result.Percent = percent;
            result.Body = part;
            return result;
        }
        private PaymentResultModel DayCreateResultModel(DayCreditInformationModel creditInformation, int index, ref decimal balance)
        {
            PaymentResultModel result = new PaymentResultModel();
            var count = creditInformation.Time / creditInformation.Step;
            result.ID = index + 1;
            decimal i = creditInformation.Rate / 100; // /100 is to deal with percents. i - monthly interest rate
            decimal power = (decimal)Math.Pow((double)(1 + i), count);
            decimal сoef = (i * power) / (power - 1);
            decimal total = сoef * creditInformation.Sum;
            decimal percent = balance / 100 * creditInformation.Rate;
            decimal part = total - percent;
            balance -= part;
            result.Balance = balance;
            result.Percent = percent;
            result.Body = part;
            return result;
        }
    }
}