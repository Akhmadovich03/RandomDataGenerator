using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using RandomDataGenerator.Service;
using System.Globalization;

namespace RandomDataGenerator.Controllers;

public class HomeController : Controller
{
    private readonly UserDataService _userDataService = new UserDataService();

    public ActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public JsonResult GenerateData(string region, int errors, int seed, int pageNumber)
    {
        int pageSize = 20;
        var data = _userDataService.GenerateData(region, errors, seed, pageNumber, pageSize);
        
        return Json(data);
    }

    [HttpPost]
    public ActionResult ExportToCSV(string region, int errors, int seed, int pageNumber)
    {
        int pageSize = pageNumber * 10;
        var data = _userDataService.GenerateData(region, errors, seed, 1, pageSize);

        using (var writer = new StringWriter())
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(data);
            return File(new System.Text.UTF8Encoding().GetBytes(writer.ToString()), "text/csv", "UserData.csv");
        }
    }
}
