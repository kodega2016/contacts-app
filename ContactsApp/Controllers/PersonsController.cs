using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace ContactsApp.Controllers;

public class PersonsController : Controller
{

    private readonly IPersonService _personService;


    public PersonsController(IPersonService personService)
    {
        _personService = personService;
    }
    
    
    // GET
    [HttpGet]
    [Route("persons/index")]
    [Route("/")]
    public IActionResult Index()
    {
        return View();
    }
}