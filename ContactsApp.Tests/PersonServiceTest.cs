using Entities;
using Microsoft.EntityFrameworkCore;
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
        _personService = new PersonService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
        _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
        _testOutputHelper = testOutputHelper;
    }

    #region AddPerson

    // When we supply null value as PersonAddRequest,
    // It should throw ArgumentNullException
    [Fact]
    public async Task AddPerson_NullPersonRequest()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                {

                    await _personService.AddPerson(null);
                });
    }

    // When we supply Name as null for PersonAddRequest
    // It should throw ArgumentNullException
    [Fact]
    public async Task AddPerson_PersonNameIsNull()
    {
        // Arrange
        PersonAddRequest? personAddRequest = new() { Name = null };

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => { await _personService.AddPerson(personAddRequest); });
    }

    // When we supply proper details,it should
    // insert the person in the person list,which includes with the newly
    // generated person id
    [Fact]
    public async Task AddPerson_ProperPersonDetails()
    {
        // Arrange
        PersonAddRequest? personRequest = new()
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
        PersonResponse response = await _personService.AddPerson(personRequest);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Id != Guid.Empty);

        List<PersonResponse> personList = await _personService.GetAllPersons();
        Assert.Contains(response, personList);
    }

    #endregion

    // If we supply null as PersonId,it should return null as PersonResponse
    [Fact]
    public async Task GetPersonByPersonID_NullPersonId()
    {
        // Arrange
        Guid? personId = null;

        // Act
        PersonResponse? personResponse = await _personService.GetPersonByPersonId(personId);

        // Assert
        Assert.Null(personResponse);
    }

    // If we supply a valid person id,it should return the valid
    // person details as PersonResponse object
    [Fact]
    public async Task GetPersonByPersonID_WithPersonId()
    {
        // Arrange
        CountryAddRequest countryAddRequest = new() { CountryName = "Nepal" };


        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            Name = "James",
            Email = "example@example.com",
            DateOfBirth = DateTime.Parse("1997-03-06"),
            Gender = GenderOptions.Male,
            Address = "Madhumalla Morang",
            ReceiveNewsLetter = true,
        };
        PersonResponse personResponse = await _personService.AddPerson(personAddRequest);

        // Act
        PersonResponse? personResponseFromService = await _personService.GetPersonByPersonId(
            personResponse?.Id ?? null
        );
        // Assert
        Assert.Equal(personResponseFromService, personResponse);
        Assert.Equal(personResponseFromService?.Id, personResponse?.Id);
    }

    #region GetAllPersons

    // The GetAllPersons should return an empty list by default
    [Fact]
    public async Task GetAllPersons_Empty()
    {
        var persons = await _personService.GetAllPersons();
        Assert.Empty(persons);
    }

    // First,we will add few persons,add then we call the GetAllPersons()
    // It should return the same persons that were added

    [Fact]
    public async Task GetAllPersons_AddFewPersons()
    {
        // Arrange
        CountryAddRequest? countryAddRequest1 = new() { CountryName = "Nepal" };
        CountryAddRequest? countryAddRequest2 = new() { CountryName = "India" };

        CountryResponse? countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
        CountryResponse? countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

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
        PersonResponse personResponse1 = await _personService.AddPerson(personAddRequest1);
        PersonResponse personResponse2 = await _personService.AddPerson(personAddRequest2);
        _testOutputHelper.WriteLine("Expected:");
        _testOutputHelper.WriteLine(personResponse1.ToString());
        _testOutputHelper.WriteLine(personResponse2.ToString());

        // Acts
        List<PersonResponse> persons = await _personService.GetAllPersons();

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
    public async Task GetFilteredPersons_EmptySearchText()
    {
        // Arrange
        CountryAddRequest? countryAddRequest1 = new() { CountryName = "Nepal" };
        CountryAddRequest? countryAddRequest2 = new() { CountryName = "India" };

        CountryResponse? countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
        CountryResponse? countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

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
        PersonResponse personResponse1 = await _personService.AddPerson(personAddRequest1);
        PersonResponse personResponse2 = await _personService.AddPerson(personAddRequest2);

        List<PersonResponse> personResponsesFromAdd = [personResponse1, personResponse2];


        // Acts
        List<PersonResponse> persons = await _personService.GetFilteredPersons(searchBy: nameof(Person.Name), "");

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
    public async Task GetFilteredPersons_SearchByName()
    {
        // Arrange
        CountryAddRequest? countryAddRequest1 = new() { CountryName = "Nepal" };
        CountryAddRequest? countryAddRequest2 = new() { CountryName = "India" };

        var countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
        var countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

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
        var personResponse1 = await _personService.AddPerson(personAddRequest1);
        var personResponse2 = await _personService.AddPerson(personAddRequest2);
        var personResponse3 = await _personService.AddPerson(personAddRequest3);
        List<PersonResponse> personResponsesFromAdd = [personResponse1, personResponse2, personResponse3];

        // Act
        var personsFromSearch = await _personService.GetFilteredPersons(nameof(Person.Name), "Jame");

        // Assert
        foreach (var personFromAdd in personResponsesFromAdd)
        {
            if (personFromAdd.Name != null)
            {
                if (personFromAdd.Name.Contains("Jame", StringComparison.OrdinalIgnoreCase))
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

    [Fact]

    public async Task GetSortedPersons()
    {
        // Arrange
        CountryAddRequest? countryAddRequest1 = new() { CountryName = "Nepal" };
        CountryAddRequest? countryAddRequest2 = new() { CountryName = "India" };

        var countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
        var countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

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
        var personResponse1 = await _personService.AddPerson(personAddRequest1);
        var personResponse2 = await _personService.AddPerson(personAddRequest2);
        var personResponse3 = await _personService.AddPerson(personAddRequest3);
        List<PersonResponse> personResponsesFromAdd = [personResponse1, personResponse2, personResponse3];
        List<PersonResponse> allPersons = await _personService.GetAllPersons();
        // Act
        var personsFromSort =
            _personService.GetSortedPersons(allPersons, nameof(Person.Name), SortOrderEnum.Descending);
        personResponsesFromAdd = personResponsesFromAdd.OrderByDescending(temp => temp.Name).ToList();

        // Assert
        for (int i = 0; i < personResponsesFromAdd.Count; i++)
        {
            Assert.Equal(personResponsesFromAdd[i], personsFromSort[i]);
        }
    }

    #endregion

    #region UpdatePerson

    // When we supply null as PersonUpdateRequest,it should throw ArgumentNullException
    [Fact]
    public async Task UpdatePerson_Empty()
    {
        // Arrange
        PersonUpdateRequest? personUpdateRequest = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => { await _personService.UpdatePerson(personUpdateRequest); });
    }

    // When we supply invalid person id,it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_InvalidId()
    {
        // Arrange
        var personUpdateRequest = new PersonUpdateRequest()
        {
            PersonId = Guid.NewGuid()
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => { await _personService.UpdatePerson(personUpdateRequest); });
    }

    // When PersonName is null,it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_NameIsNull()
    {
        // Arrange
        var countryAddRequest = new CountryAddRequest()
        {
            CountryName = "Nepal"
        };
        var countryResponse = await _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest personAddRequest = new()
        {
            Name = "James",
            CountryId = countryResponse?.CountryId,
            Email = "example@example.com",
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Parse("1997-03-06"),
        };

        var personResponse = await _personService.AddPerson(personAddRequest);
        var personUpdateRequest = personResponse.ToPersonUpdateRequest();
        personUpdateRequest.Name = null;


        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () =>
                {
                    // Act
                    await _personService.UpdatePerson(personUpdateRequest);
                });
    }

    // First,Add a new person and try to update the person name and email
    [Fact]
    public async Task UpdatePerson_ValidData()
    {
        // Arrange
        var countryAddRequest = new CountryAddRequest()
        {
            CountryName = "Nepal"
        };
        var countryResponse = await _countriesService.AddCountry(countryAddRequest);
        PersonAddRequest personAddRequest = new()
        {
            Name = "James",
            CountryId = countryResponse?.CountryId,
            Email = "example@example.com",
            Gender = GenderOptions.Male,
            DateOfBirth = DateTime.Parse("1997-03-06"),
        };

        var personResponse = await _personService.AddPerson(personAddRequest);
        var personUpdateRequest = personResponse.ToPersonUpdateRequest();
        personUpdateRequest.Name = "Jack";
        personUpdateRequest.Email = "james@example.com";

        // Act
        var updatedPerson = await _personService.UpdatePerson(personUpdateRequest);
        var updatedPersonFromResponse = await _personService.GetPersonByPersonId(updatedPerson?.Id);

        Assert.Equal(updatedPersonFromResponse, updatedPerson);
        Assert.NotNull(updatedPerson);
    }

    #endregion

    #region DeletePerson

    // When we try to delete without passing the id,it should throw ArgumentNullException
    [Fact]
    public async Task DeletePerson_Null()
    {
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                {
                    // Act
                    await _personService.DeletePerson(null);
                });
    }


    [Fact]
    public async Task DeletePerson_NotFound()
    {
        // Arrange
        var personID = Guid.NewGuid();
        var personResponse = await _personService.DeletePerson(personID);
        Assert.False(personResponse);
    }

    // When we try to delete non-existent item,it should return false
    [Fact]
    public async Task DeletePerson_NonExistingPerson()
    {
        // Arrange
        var personId = Guid.NewGuid();

        // Act
        var response = await _personService.DeletePerson(personId);

        // Assert
        Assert.False(response);
    }

    // When we delete the available person it should delete
    // And return true

    [Fact]
    public async Task DeletePerson_ValidPerson()
    {
        // Arrange
        var countryAddRequest = new CountryAddRequest()
        {
            CountryName = "Nepal"
        };
        var countryResponse = await _countriesService.AddCountry(countryAddRequest);
        var personAddRequest = new PersonAddRequest()
        {
            Name = "James",
            Email = "james@example.com",
            Gender = GenderOptions.Male,
            Address = "Biratnagar",
            CountryId = countryResponse?.CountryId,
            ReceiveNewsLetter = true,
            DateOfBirth = DateTime.Parse("1997-03-06")
        };

        var addedPerson = await _personService.AddPerson(personAddRequest);

        // Act
        var result = await _personService.DeletePerson(addedPerson?.Id);
        // Assert
        Assert.True(result);

    }

    #endregion
};
