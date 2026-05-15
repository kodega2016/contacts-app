using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ContactsApp.Controllers;

[Route("persons")]
public class PersonsController : Controller
{
    private readonly IPersonService _personService;
    private readonly ICountriesService _countriesService;

    public PersonsController(IPersonService personService, ICountriesService countriesService)
    {
        _personService = personService;
        _countriesService = countriesService;
    }

    // GET
    [HttpGet]
    // [Route("persons/index")]
    [Route("[action]")]
    [Route("/")]
    public IActionResult Index(
        string searchBy,
        string? searchString,
        string sortBy = nameof(PersonResponse.Name),
        SortOrderEnum sortOrder = SortOrderEnum.Descending
    )
    {
        var searchFields = new Dictionary<string, string>
        {
            { nameof(PersonResponse.Name), "Name" },
            { nameof(PersonResponse.Email), "Email Address" },
            { nameof(PersonResponse.DateOfBirth), "Date Of Birth" },
            { nameof(PersonResponse.Gender), "Gender" },
            { nameof(PersonResponse.CountryId), "Country Id" },
            { nameof(PersonResponse.Address), "Address" }
        };
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

    // [Route("persons/create")]
    [Route("[action]")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var countries =await _countriesService.GetCountries();
        ViewBag.Countries = countries.Select(temp =>
        {
            return new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryId.ToString()
            };

        }).ToList();



        return View();
    }

    // [Route("persons/create")]
    [Route("[action]")]
    [HttpPost]
    public async Task<IActionResult> Create(PersonAddRequest request)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            var countries = await _countriesService.GetCountries();

            ViewBag.Countries = countries.Select(temp =>
            {
                return new SelectListItem()
                {
                    Text = temp.CountryName,
                    Value = temp.CountryId.ToString()
                };

            }).ToList();
            return View();

        }

        PersonResponse personResponse = _personService.AddPerson(request);
        return RedirectToAction("Index", "Persons");
    }


    [Route("[action]/{id}")]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        PersonResponse? personResponse =  _personService.GetPersonByPersonId(id);
        if (personResponse == null)
        {
            return RedirectToAction("Index", "Persons");
        }
        var countries = await _countriesService.GetCountries();
        ViewBag.Countries = countries.Select(temp =>
        {
            return new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryId.ToString(),
            };
        });

        PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
        return View(personUpdateRequest);
    }

    [HttpPost]
    [Route("[action]/{personId}")]
    public async Task<IActionResult> Edit(Guid personId, PersonUpdateRequest request)
    {
        PersonResponse? personResponse = _personService.GetPersonByPersonId(personId);

        if (ModelState.IsValid)
        {
            PersonResponse? updatedPerson = _personService.UpdatePerson(request);
            return RedirectToAction("Index", "Persons");
        }
        else
        {
            var countries =await _countriesService.GetCountries();
            ViewBag.Countries = countries.Select(temp =>
            {
                return new SelectListItem()
                {
                    Text = temp.CountryName,
                    Value = temp.CountryId.ToString(),
                };
            });

            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToList();
        }

        return View();
    }

    [HttpGet]
    [Route("[action]/{personId}")]
    public IActionResult Delete(Guid personId)
    {
        PersonResponse? personResponse = _personService.GetPersonByPersonId(personId);
        if (personResponse == null)
        {
            return RedirectToAction("Index", "Persons");

        }

        return View(personResponse);
    }


    [HttpPost]
    [Route("[action]/{personId}")]
    public IActionResult Delete(Guid personId, PersonUpdateRequest request)
    {
        _personService.DeletePerson(personId);
        return RedirectToAction("Index");

    }
}

