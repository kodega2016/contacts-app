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
        return _persons.Select(person => person.ToPersonResponse()).ToList();
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
