using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class PersonService : IPersonService
{
    private readonly List<Person> _persons;
    private readonly ICountriesService _countriesService;

    public PersonService(ICountriesService countriesService)
    {
        _persons = new List<Person>() { };
        _countriesService = countriesService;
    }




    public PersonResponse AddPerson(PersonAddRequest? request)
    {
        // check if PersonAddRequest is not null
        ArgumentNullException.ThrowIfNull(request);


        // Model Validations
        ValidationContext validationContext = new ValidationContext(request);
        List<ValidationResult> validationResults = new List<ValidationResult>();


        bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);
        if (!isValid)
        {
            throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
        }

        // Validate Name
        // if (string.IsNullOrWhiteSpace(request.Name))
        // {
        //     throw new ArgumentNullException(nameof(request));
        // }

        // Convert personAddRequest into Person type
        Person person = request.ToPerson();
        _persons.Add(person);

        // Generate PersonId
        person.PersonId = Guid.NewGuid();

        // Convert the person object into PersonResponse type
        return ConvertPersonToPersonResponse(person);
    }

    public List<PersonResponse> GetAllPersons()
    {
        return _persons.Select(person => ConvertPersonToPersonResponse(person)).ToList();
    }

    public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
    {
        List<PersonResponse> allPersons = GetAllPersons();
        List<PersonResponse> matchingPersons = allPersons;


        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty((searchString))) return matchingPersons;

        switch (searchBy)
        {
            case nameof(Person.Name):
                matchingPersons = allPersons.Where(temp => temp.Name != null && (!string.IsNullOrEmpty(temp.Name) ||
                    temp.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))).ToList();
                break;

            case nameof(Person.Email):
                matchingPersons = allPersons.Where(temp => temp.Email != null && (!string.IsNullOrEmpty(temp.Email) ||
                    temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase))).ToList();
                break;

            case nameof(Person.DateOfBirth):
                matchingPersons = allPersons.Where(temp => temp.DateOfBirth != null && (
                    temp.DateOfBirth.Value.ToString("dd MMMM yyyy")
                        .Contains(searchString, StringComparison.OrdinalIgnoreCase))).ToList();
                break;

            case nameof(Person.Gender):
                matchingPersons = allPersons.Where(temp => temp.Gender != null && (
                    temp.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase))).ToList();
                break;

            case nameof(Person.CountryId):
                matchingPersons = allPersons.Where(temp => temp.Country != null && (
                    temp.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase))).ToList();
                break;

            case nameof(Person.Address):
                matchingPersons = allPersons.Where(temp => temp.Address != null && (
                    temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase))).ToList();
                break;

            default:
                matchingPersons = allPersons;
                break;
        }

        return matchingPersons;
    }

    public PersonResponse? GetPersonByPersonId(Guid? personId)
    {
        if (personId == null) return null;
        Person? person = _persons.FirstOrDefault(person => person.PersonId == personId);
        if (person == null) return null;
        return ConvertPersonToPersonResponse(person);
    }

    private PersonResponse ConvertPersonToPersonResponse(Person person)
    {
        PersonResponse personResponse = person.ToPersonResponse();
        personResponse.Country = _countriesService.GetCountryByCountryId(person.CountryId)?.CountryName;
        return personResponse;
    }
}
