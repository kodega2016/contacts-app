using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;

namespace ContactsApp.Tests;

public class PersonServiceTest
{
    private readonly IPersonService _personService;
    private readonly ICountriesService _countriesService;


    public PersonServiceTest()
    {
        _countriesService = new CountriesService();
        _personService = new PersonService(_countriesService);

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
        PersonAddRequest? personAddRequest = new PersonAddRequest()
        {
            Name = null
        };

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
            BirthDate = DateTime.Parse("1997-03-06"),
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
        CountryAddRequest countryAddRequest = new CountryAddRequest()
        {
            CountryName = "Nepal"
        };
        CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

        PersonAddRequest personAddRequest = new PersonAddRequest()
        {
            Name = "James",
            Email = "example@example.com",
            BirthDate = DateTime.Parse("1997-03-06"),
            Gender = GenderOptions.Male,
            Address = "Madhumalla Morang",
            CountryId = countryResponse.CountryId,
            ReceiveNewsLetter = true,
        };
        PersonResponse personResponse = _personService.AddPerson(personAddRequest);

        // Act
        PersonResponse? personResponseFromService = _personService.GetPersonByPersonId(personResponse?.Id??null);
        // Assert
        Assert.Equal(personResponseFromService, personResponse);
        Assert.Equal(personResponseFromService?.Id, personResponse?.Id);
    }


    #region

    #endregion
}
;
