using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace ContactsApp.Controllers;

[Route("[controller]")]

public class CountriesController : Controller
{

    private readonly ICountriesService _countriesService;


    public CountriesController(ICountriesService countriesService)
    {
        _countriesService = countriesService;
    }




    [HttpGet]
    [Route("upload-from-excel")]
    public IActionResult UploadFromExcel()
    {
        return View("upload-from-excel");
    }



    [HttpPost]
    [Route("upload-from-excel")]

    public async Task<IActionResult> UploadFromExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ViewBag.ErrorMessage = "Please select an xlsx file";
            return View("upload-from-excel");
        }

        if (!Path.GetExtension(file!.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            ViewBag.ErrorMessage = "Unsupported file,xlsx file is expected";
        }

        var countriesAdded = await _countriesService.UploadCountriesFromExcelFile(file);
        ViewBag.Message = $"{countriesAdded} Countires are uploaded.";
        return View("upload-from-excel");
    }
}