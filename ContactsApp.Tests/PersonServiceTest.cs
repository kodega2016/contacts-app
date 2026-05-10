using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace ContactsApp.Tests;

public class PersonServiceTest
{
    private readonly IPersonService _personService;
    private readonly ICountriesService _countriesService;
    private readonly ITestOutputHelper _testOutputHelper;

    public PersonServiceTest(ITestOutputHelper testOutputHelper)
    {
        _countriesService = new CountriesService();
        _personService = new PersonService(_countriesService);
        _testOutputHelper = testOutputHelper;
    }

    #region AddPerson
    // When we supply null value as PersonAddRequest,
    // It should throw ArgumentNullException
    [Fact]
    public void AddPerson_NullPersonRequest()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            _personService.AddPerson(null);
        });
    }

    // When we supply Name as null for PersonAddRequest
    // It should throw ArgumentNullException
    [Fact]
    public void AddPerson_PersonNameIsNull()
    {
        // Arrange
        PersonAddRequest? personAddRequest = new PersonAddRequest() { Name = null };

        // Act and Assert
        Assert.Throws<ArgumentException>(() =>
        {
            _personService.AddPerson(personAddRequest);
        });
    }

    // When we supply proper details,it should
    // insert the person in the person list,which includes with the newly
    // generated person id
    [Fact]
    public void AddPerson_ProperPersonDetails()
    {
        // Arrange
        PersonAddRequest? personRequest = new PersonAddRequest()
        {
            Name = "James",
            Email = "example@example.com",
            DateOfBirth = DateTime.Parse("1997-03-06"),
            Gender = GenderOptions.Male,
            Address = "Madhumalla Morang",
            CountryId = Guid.NewGuid(),
            ReceiveNewsLetter = true,
        };

        // Acts
        PersonResponse response = _personService.AddPerson(personRequest);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Id != Guid.Empty);

        List<PersonResponse> personList = _personService.GetAllPersons();
        Assert.Contains(response, personList);
    }
    #endregion
    // If we supply null as PersonId,it should return null as PersonResponse
    [Fact]
    public void GetPersonByPersonID_NullPersonId()
    {
        // Arrange
        Guid? personId = null;

        // Act
        PersonResponse? personResponse = _personService.GetPersonByPersonId(personId);

        // Assert
        Assert.Null(personResponse);
    }

    // If we supply a valid person id,it should return the valid
    // person details as PersonResponse object
    [Fact]
    public void GetPersonByPersonID_WithPersonId()
    {
        // Arrange
        CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Nepal" };
        CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            Name = "James",
            Email = "example@example.com",
            DateOfBirth = DateTime.Parse("1997-03-06"),
            Gender = GenderOptions.Male,
            Address = "Madhumalla Morang",
            CountryId = countryResponse.CountryId,
            ReceiveNewsLetter = true,
        };
        PersonResponse personResponse = _personService.AddPerson(personAddRequest);

        // Act
        PersonResponse? personResponseFromService = _personService.GetPersonByPersonId(
            personResponse?.Id ?? null
        );
        // Assert
        Assert.Equal(personResponseFromService, personResponse);
        Assert.Equal(personResponseFromService?.Id, personResponse?.Id);
    }

    #region GetAllPersons
    // The GetAllPersons should return an empty list by default
    [Fact]
    public void GetAllPersons_Empty()
    {
        var persons = _personService.GetAllPersons();
        Assert.Empty(persons);
    }

    // First,we will add few persons,add then we call the GetAllPersons()
    // It should return the same persons that were added

    [Fact]
    public void GetAllPersons_AddFewPersons()
    {
        // Arrange
        CountryAddRequest? countryAddRequest1 = new() { CountryName = "Nepal" };
        CountryAddRequest? countryAddRequest2 = new() { CountryName = "India" };

        CountryResponse? countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
        CountryResponse? countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

        PersonAddRequest personAddRequest1 = new()
        {
            Name = "James",
            Email = "example@example.com",
            DateOfBirth = DateTime.Parse("1997-03-06"),
            Gender = GenderOptions.Male,
            Address = "Madhumalla Morang",
            CountryId = countryResponse1.CountryId,
            ReceiveNewsLetter = true,
        };

        PersonAddRequest personAddRequest2 = new()
        {
            Name = "Yash",
            Email = "yash@example.com",
            DateOfBirth = DateTime.Parse("1997-03-06"),
            Gender = GenderOptions.Male,
            Address = "New Delli",
            CountryId = countryResponse2.CountryId,
            ReceiveNewsLetter = false,
        };
        PersonResponse personResponse1 = _personService.AddPerson(personAddRequest1);
        PersonResponse personResponse2 = _personService.AddPerson(personAddRequest2);
        _testOutputHelper.WriteLine("Expected:");
        _testOutputHelper.WriteLine(personResponse1.ToString());
        _testOutputHelper.WriteLine(personResponse2.ToString());

        // Acts
        List<PersonResponse> persons = _personService.GetAllPersons();

        _testOutputHelper.WriteLine("Actual:");
        foreach (var person in persons)
        {
            _testOutputHelper.WriteLine(person.ToString());
        }

        // Assert
        Assert.Contains(personResponse1, persons);
        Assert.Contains(personResponse2, persons);
        Assert.True(persons.Count == 2);
    }
    #endregion


    #region GetFilteredPersons
    // If the search text is empty and search by is Name it should return all persons
    [Fact]
    public void GetFilteredPersons_EmptySearchText()
    {
        // Arrange
        CountryAddRequest? countryAddRequest1 = new() { CountryName = "Nepal" };
        CountryAddRequest? countryAddRequest2 = new() { CountryName = "India" };

        CountryResponse? countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
        CountryResponse? countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

        PersonAddRequest personAddRequest1 = new()
        {
            Name = "James",
            Email = "example@example.com",
            DateOfBirth = DateTime.Parse("1997-03-06"),
            Gender = GenderOptions.Male,
            Address = "Madhumalla Morang",
            CountryId = countryResponse1.CountryId,
            ReceiveNewsLetter = true,
        };

        PersonAddRequest personAddRequest2 = new()
        {
            Name = "Yash",
            Email = "yash@example.com",
            DateOfBirth = DateTime.Parse("1997-03-06"),
            Gender = GenderOptions.Male,
            Address = "New Delli",
            CountryId = countryResponse2.CountryId,
            ReceiveNewsLetter = false,
        };
        PersonResponse personResponse1 = _personService.AddPerson(personAddRequest1);
        PersonResponse personResponse2 = _personService.AddPerson(personAddRequest2);

        List<PersonResponse> personResponsesFromAdd = [personResponse1, personResponse2];
        
        
        // Acts
        List<PersonResponse> persons = _personService.GetFilteredPersons(searchBy:nameof(Person.Name),"");
        
        // Assert
        Assert.Equal(persons, personResponsesFromAdd);
        Assert.Equal(persons, personResponsesFromAdd);

        foreach (var person in persons)
        {
            Assert.Contains(person, persons);
        }

    }
    
    
    // First we will add few persons;and then we will search based on
    // person name with some search string,it should return the matching
    // persons
    [Fact]
    public void GetFilteredPersons_SearchByName()
    {
        // Arrange
        CountryAddRequest? countryAddRequest1 = new() { CountryName = "Nepal" };
        CountryAddRequest? countryAddRequest2 = new() { CountryName = "India" };

        var countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
        var countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

        PersonAddRequest personAddRequest1 = new()
        {
            Name = "James",
            Email = "example@example.com",
            DateOfBirth = DateTime.Parse("1997-03-06"),
            Gender = GenderOptions.Male,
            Address = "Madhumalla Morang",
            CountryId = countryResponse1.CountryId,
            ReceiveNewsLetter = true,
        };

        PersonAddRequest personAddRequest2 = new()
        {
            Name = "Yash",
            Email = "yash@example.com",
            DateOfBirth = DateTime.Parse("1997-03-06"),
            Gender = GenderOptions.Male,
            Address = "New Delli",
            CountryId = countryResponse2.CountryId,
            ReceiveNewsLetter = false,
        };
        
        PersonAddRequest personAddRequest3 = new()
        {
            Name = "James Vogan",
            Email = "vogan@example.com",
            DateOfBirth = DateTime.Parse("1997-03-06"),
            Gender = GenderOptions.Male,
            Address = "Biratnagar",
            CountryId = countryResponse2.CountryId,
            ReceiveNewsLetter = true,
        };
        var personResponse1 = _personService.AddPerson(personAddRequest1);
        var personResponse2 = _personService.AddPerson(personAddRequest2);
        var personResponse3 = _personService.AddPerson(personAddRequest3);
        List<PersonResponse> personResponsesFromAdd = [personResponse1, personResponse2,personResponse3];
        
        // Act
        var personsFromSearch = _personService.GetFilteredPersons(nameof(Person.Name), "Jame");

        // Assert
        foreach (var personFromAdd in personResponsesFromAdd)
        {
            if (personFromAdd.Name != null)
            {
                if(personFromAdd.Name.Contains("Jame",StringComparison.OrdinalIgnoreCase))
                {
                    Assert.Contains(personFromAdd, personsFromSearch);
                }
            }
            
        }
    }

    #endregion
    
    #region GetSortedPersons
    
    // When we sort based on the Name in DESC,it should return 
    // persons list in descending on Name

    public void GetSortedPersons()
    {
        // Arrange
        CountryAddRequest? countryAddRequest1 = new() { CountryName = "Nepal" };
        CountryAddRequest? countryAddRequest2 = new() { CountryName = "India" };

        var countryResponse1 = _countriesService.AddCountry(countryAddRequest1);
        var countryResponse2 = _countriesService.AddCountry(countryAddRequest2);

        PersonAddRequest personAddRequest1 = new()
        {
            Name = "James",
            Email = "example@example.com",
            DateOfBirth = DateTime.Parse("1997-03-06"),
            Gender = GenderOptions.Male,
            Address = "Madhumalla Morang",
            CountryId = countryResponse1.CountryId,
            ReceiveNewsLetter = true,
        };

        PersonAddRequest personAddRequest2 = new()
        {
            Name = "Yash",
            Email = "yash@example.com",
            DateOfBirth = DateTime.Parse("1997-03-06"),
            Gender = GenderOptions.Male,
            Address = "New Delli",
            CountryId = countryResponse2.CountryId,
            ReceiveNewsLetter = false,
        };
        
        PersonAddRequest personAddRequest3 = new()
        {
            Name = "James Vogan",
            Email = "vogan@example.com",
            DateOfBirth = DateTime.Parse("1997-03-06"),
            Gender = GenderOptions.Male,
            Address = "Biratnagar",
            CountryId = countryResponse2.CountryId,
            ReceiveNewsLetter = true,
        };
        var personResponse1 = _personService.AddPerson(personAddRequest1);
        var personResponse2 = _personService.AddPerson(personAddRequest2);
        var personResponse3 = _personService.AddPerson(personAddRequest3);
        List<PersonResponse> personResponsesFromAdd = [personResponse1, personResponse2,personResponse3];
        List<PersonResponse> allPersons = _personService.GetAllPersons();
        // Act
        var personsFromSort = _personService.GetSortedPersons(allPersons, nameof(Person.Name), SortOrderEnum.Descending);
        personResponsesFromAdd = personResponsesFromAdd.OrderByDescending(temp => temp.Name).ToList();

        // Assert
        for (int i = 0; i < personResponsesFromAdd.Count; i++)
        {
            Assert.Equal(personResponsesFromAdd[i],personsFromSort[i]);
        }
    }
    
    #endregion

    #region UpdatePerson
    // When we supply null as PersonUpdateRequest,it should throw ArgumentNullException
    [Fact]
    public void UpdatePerson_Empty()
    {
        // Arrange
        PersonUpdateRequest? personUpdateRequest = null;
        
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            _personService.UpdatePerson(personUpdateRequest);
        });
    }
    
    // When we supply invalid person id,it should throw ArgumentException
    [Fact]
    public void UpdatePerson_InvalidId()
    {
        // Arrange
        var personUpdateRequest = new PersonUpdateRequest()
        {
            PersonId = Guid.NewGuid()
        };
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
        {
            _personService.UpdatePerson(personUpdateRequest);
        });
    }
    
    // When PersonName is null,it should throw ArgumentException
    [Fact]
    public void UpdatePerson_NameIsNull()
    {
        // Arrange
        var countryAddRequest = new CountryAddRequest()
        {
            CountryName = "Nepal"
        };
        var countryResponse = _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest personAddRequest = new()
        {
            Name = "James",
            CountryId = countryResponse?.CountryId,
            Email = "example@example.com",
            Gender = GenderOptions.Male,
        };

        var personResponse = _personService.AddPerson(personAddRequest);
        var personUpdateRequest = personResponse.ToPersonUpdateRequest();
        personUpdateRequest.Name = null;


        // Assert
        Assert.Throws<ArgumentException>(() =>
        {
            // Act
            _personService.UpdatePerson(personUpdateRequest);
        });
    }
    
    // First,Add a new person and try to update the person name and email
    [Fact]
    public void UpdatePerson_ValidData()
    {
        // Arrange
        var countryAddRequest = new CountryAddRequest()
        {
            CountryName = "Nepal"
        };
        var countryResponse = _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest personAddRequest = new()
        {
            Name = "James",
            CountryId = countryResponse?.CountryId,
            Email = "example@example.com",
            Gender = GenderOptions.Male,
        };

        var personResponse = _personService.AddPerson(personAddRequest);
        var personUpdateRequest = personResponse.ToPersonUpdateRequest();
        personUpdateRequest.Name = "Jack";
        personUpdateRequest.Email = "james@example.com";

        // Act
        var updatedPerson = _personService.UpdatePerson(personUpdateRequest);
        var updatedPersonFromResponse = _personService.GetPersonByPersonId(updatedPerson?.Id);

        Assert.Equal(updatedPersonFromResponse, updatedPerson);
        Assert.NotNull(updatedPerson);
    }

    #endregion
};
