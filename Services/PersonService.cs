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

    public PersonService(ICountriesService countriesService,bool initialize=true)
    {
        _persons = new List<Person>() { };
        _countriesService = countriesService;

        if (initialize)
        {
            _persons.AddRange(new List<Person>()
            {
                new Person()
                {
                    PersonId = Guid.Parse("F3444926-80FB-416A-8BCC-F27CE006060C"),
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    DateOfBirth = new DateTime(1995, 5, 12),
                    Gender = "Male",
                    CountryId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Address = "123 Main Street, New York",
                    ReceiveNewsLetter = true
                },

                new Person()
                {
                    PersonId = Guid.Parse("0E9F57DC-63F1-4F59-8A04-577793E5FDDA"),
                    Name = "Emma Watson",
                    Email = "emma.watson@example.com",
                    DateOfBirth = new DateTime(1998, 8, 25),
                    Gender = "Female",
                    CountryId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Address = "45 Queen Street, London",
                    ReceiveNewsLetter = false
                },

                new Person()
                {
                    PersonId = Guid.Parse("8084A652-F83A-47CA-9885-1263D8676334"),
                    Name = "Michael Smith",
                    Email = "michael.smith@example.com",
                    DateOfBirth = new DateTime(1989, 11, 3),
                    Gender = "Male",
                    CountryId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Address = "78 George Street, Sydney",
                    ReceiveNewsLetter = true
                },

                new Person()
                {
                    PersonId = Guid.Parse("33A7C9B8-7266-4691-BF36-21D6FFD06DBB"),
                    Name = "Sophia Johnson",
                    Email = "sophia.johnson@example.com",
                    DateOfBirth = new DateTime(2001, 2, 14),
                    Gender = "Female",
                    CountryId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Address = "12 Sunset Blvd, Los Angeles",
                    ReceiveNewsLetter = false
                },

                new Person()
                {
                    PersonId = Guid.NewGuid(),
                    Name = "David Brown",
                    Email = "david.brown@example.com",
                    DateOfBirth = new DateTime(1993, 7, 9),
                    Gender = "Male",
                    CountryId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Address = "99 King Street, Toronto",
                    ReceiveNewsLetter = true
                }
            });
        }
        
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
            case nameof(PersonResponse.Name):
                matchingPersons=   allPersons.Where(temp =>
                    temp.Name?.Contains(searchString,
                        StringComparison.OrdinalIgnoreCase) == true
                ).ToList();
                break;

            case nameof(PersonResponse.Email):
                matchingPersons = allPersons.Where(temp =>
                    temp.Email?.Contains(searchString,
                        StringComparison.OrdinalIgnoreCase) == true
                ).ToList();
                break;

            case nameof(Person.DateOfBirth):
                matchingPersons = allPersons.Where(temp =>
                    temp.DateOfBirth?.ToString("dd MMMM yyyy")
                        .Contains(searchString,
                            StringComparison.OrdinalIgnoreCase) == true
                ).ToList();
                break;

            case nameof(Person.Gender):
                matchingPersons = allPersons.Where(temp =>
                    temp.Gender?.Contains(searchString,
                        StringComparison.OrdinalIgnoreCase) == true
                ).ToList();
                break;

            case nameof(Person.CountryId):
                matchingPersons = allPersons.Where(temp =>
                    temp.Country?.Contains(searchString,
                        StringComparison.OrdinalIgnoreCase) == true
                ).ToList();
                break;

            case nameof(Person.Address):
                matchingPersons = allPersons.Where(temp =>
                    temp.Address?.Contains(searchString,
                        StringComparison.OrdinalIgnoreCase) == true
                ).ToList();
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

    public bool DeletePerson(Guid? personId)
    {
        if (personId == null) throw new ArgumentNullException(nameof(personId));
        var person = _persons.FirstOrDefault(person => person.PersonId == personId);
        if (person == null) return false;

        _persons.RemoveAll(p => p.PersonId == personId);
        return true;
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
