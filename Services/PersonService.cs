using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

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
        ValidationHelpers.ModelValidation(request);
        // ValidationContext validationContext = new ValidationContext(request);
        // List<ValidationResult> validationResults = new List<ValidationResult>();
        //
        //
        // bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);
        // if (!isValid)
        // {
        //     throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
        // }

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

    public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderEnum sortOrder)
    {
        if(string.IsNullOrEmpty(sortBy)){
            return allPersons;
        }

        List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
        {
            (nameof(PersonResponse.Name), SortOrderEnum.Ascending) =>
                allPersons.OrderBy(temp => temp.Name, StringComparer.OrdinalIgnoreCase).ToList(),

            (nameof(PersonResponse.Name), SortOrderEnum.Descending) =>
                allPersons.OrderByDescending(temp => temp.Name, StringComparer.OrdinalIgnoreCase).ToList(),
                
                    (nameof(PersonResponse.Email), SortOrderEnum.Ascending) =>
            allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
            
            (nameof(PersonResponse.Email), SortOrderEnum.Descending) =>
                allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
            
            
            (nameof(PersonResponse.DateOfBirth), SortOrderEnum.Ascending) =>
                allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),
            
            (nameof(PersonResponse.DateOfBirth), SortOrderEnum.Descending) =>
                allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),
            
            
             
            (nameof(PersonResponse.Age), SortOrderEnum.Ascending) =>
                allPersons.OrderBy(temp => temp.Age).ToList(),
            
            (nameof(PersonResponse.Age), SortOrderEnum.Descending) =>
                allPersons.OrderByDescending(temp => temp.Age).ToList(),

            
            (nameof(PersonResponse.Gender), SortOrderEnum.Ascending) =>
                allPersons.OrderBy(temp => temp.Gender).ToList(),
            
            (nameof(PersonResponse.Gender), SortOrderEnum.Descending) =>
                allPersons.OrderByDescending(temp => temp.Gender).ToList(),
            
            (nameof(PersonResponse.Country), SortOrderEnum.Ascending) =>
                allPersons.OrderBy(temp => temp.Country).ToList(),
            
            (nameof(PersonResponse.Country), SortOrderEnum.Descending) =>
                allPersons.OrderByDescending(temp => temp.Country).ToList(),
            
               
            (nameof(PersonResponse.Address), SortOrderEnum.Descending) =>
                allPersons.OrderByDescending(temp => temp.Address).ToList(),
            
            
            (nameof(PersonResponse.ReceiveNewsLetter), SortOrderEnum.Ascending) =>
                allPersons.OrderByDescending(temp => temp.ReceiveNewsLetter).ToList(),
            
               
            (nameof(PersonResponse.ReceiveNewsLetter), SortOrderEnum.Descending) =>
                allPersons.OrderByDescending(temp => temp.ReceiveNewsLetter).ToList(),
            
            _=>allPersons
            
            
            
        };

        return sortedPersons;

    }

    public PersonResponse? UpdatePerson(PersonUpdateRequest? request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (request.Name == null) throw new ArgumentException(nameof(request.Name));


        // Validation
        ValidationHelpers.ModelValidation(request);

        // get matching person to update
        var matchingPerson = _persons.FirstOrDefault(person => person.PersonId == request.PersonId);
        if (matchingPerson == null) throw new ArgumentException("Given person id does not exist");
        
        // update all the details
        matchingPerson.Name=request.Name;
        matchingPerson.Email=request.Email;
        matchingPerson.DateOfBirth=request.DateOfBirth;
        matchingPerson.Gender=request.Gender.ToString();
        matchingPerson.CountryId=request.CountryId;
        matchingPerson.Address=request.Address;
        matchingPerson.ReceiveNewsLetter=request.ReceiveNewsLetter;
        return matchingPerson.ToPersonResponse();
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
