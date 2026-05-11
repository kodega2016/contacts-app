using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

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
    public IActionResult Index(
        string searchBy,
        string? searchString,
        string sortBy = nameof(PersonResponse.Name),
        SortOrderEnum sortOrder = SortOrderEnum.Descending
    )
    {
        var searchFields = new Dictionary<string, string>();
        searchFields.Add(nameof(PersonResponse.Name), "Name");
        searchFields.Add(nameof(PersonResponse.Email), "Email Address");
        searchFields.Add(nameof(PersonResponse.DateOfBirth), "Date Of Birth");
        searchFields.Add(nameof(PersonResponse.Gender), "Gender");
        searchFields.Add(nameof(PersonResponse.CountryId), "Country Id");
        searchFields.Add(nameof(PersonResponse.Address), "Address");
        ViewBag.SearchFields = searchFields;
        ViewBag.searchBy = searchBy;
        ViewBag.searchString = searchString;

        // Implement search filter
        List<PersonResponse> persons = _personService.GetFilteredPersons(searchBy, searchString);

        // Implement sorting feature
        persons = _personService.GetSortedPersons(persons, sortBy, sortOrder);

        // Setting up sorting data to the ViewBag
        ViewBag.SortBy = sortBy;
        ViewBag.SortOrder = sortOrder;
        return View(persons);
    }
}
