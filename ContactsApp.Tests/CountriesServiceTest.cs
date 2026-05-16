using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace ContactsApp.Tests;

public class CountriesServiceTest
{
    private readonly ICountriesService _countriesService;

    public CountriesServiceTest()
    {
        _countriesService = new CountriesService(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
    }

    #region AddCountry
    // When CountryAddRequest is null,it should throw ArgumentNullException
    [Fact]
    public async Task AddCountry_NullCountryAddRequest()
    {
        // Arrange
        CountryAddRequest? request = null;
        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                {
                    // Act
                    await _countriesService.AddCountry(request);
                });

    }
    // When the CountryName is null,it should throw ArgumentException
    [Fact]
    public async Task AddCountry_CountryNameIsNull()
    {
        CountryAddRequest? request = new()
        {
            CountryName = null
        };

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await _countriesService.AddCountry(request);
        });
    }
    // When the CountryName is duplicate,it should throw ArgumentException
    [Fact]
    public void AddCountry_DuplicateCountryName()
    {
        CountryAddRequest? request1 = new()
        {
            CountryName = "Nepal"
        };

        CountryAddRequest? request2 = new()
        {
            CountryName = "Nepal"
        };

        Assert.Throws<ArgumentException>(() =>
        {

            _countriesService.AddCountry(request1);
            _countriesService.AddCountry(request2);
        });


    }
    // When you supply proper country name,it should insert(add) the country to the existing list of countries
    [Fact]
    public async Task AddCountry_ProperCountryDetails()
    {
        // Arrange
        CountryAddRequest? request = new()
        {
            CountryName = "Nepal"
        };
        // Act
        CountryResponse response = await _countriesService.AddCountry(request);

        // Assert
        Assert.True(response.CountryId != Guid.Empty);
    }

    #endregion

    #region GetAllCountries

    [Fact]
    public async Task GetAllCountries_EmptyList()
    {
        List<CountryResponse> _countries = await _countriesService.GetCountries();
        Assert.Empty(_countries);
    }

    [Fact]
    public async Task GetAllCountries_AddFewCountries()
    {
        // Arrange
        List<CountryAddRequest> _country_request_list = new()
        {
            new CountryAddRequest()
            {
                CountryName = "Nepal",
            },
            new CountryAddRequest()
            {
                CountryName = "India",
            }
        };

        // Act
        List<CountryResponse> countries_list_from_add_country = new();
        foreach (CountryAddRequest _country_request in _country_request_list)
        {
            countries_list_from_add_country.Add(await _countriesService.AddCountry(_country_request));
        }

        // Assert
        List<CountryResponse> _countries = await _countriesService.GetCountries();
        Assert.NotEmpty(_countries);

        // read each element from countries_list_from_add_country
        foreach (CountryResponse expected_country in countries_list_from_add_country)
        {
            Assert.Contains(expected_country, countries_list_from_add_country);
        }
    }
    #endregion

    #region GetCountryByCountryID

    [Fact]
    public async Task GetCountryByCountryID_NullCountryID()
    {
        Guid? countryID = null;
        CountryResponse? _response = await _countriesService.GetCountryByCountryId(countryID);
        Assert.Null(_response);
    }

    [Fact]
    public async Task GetCountryByCountryID_ValidCountryID()
    {
        // Arrange
        CountryAddRequest? request = new()
        {
            CountryName = "Nepal"
        };


        CountryResponse _fromResponse = await _countriesService.AddCountry(request);

        // Act
        CountryResponse? response = await _countriesService.GetCountryByCountryId(_fromResponse.CountryId);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(_fromResponse, response);
        Assert.Equal(_fromResponse.CountryId, response.CountryId);
    }

    [Fact]
    public async Task GetCountryByCountryID_InvalidCountryID()
    {
        var randomCountryID = Guid.NewGuid();
        CountryResponse? response = await _countriesService.GetCountryByCountryId(randomCountryID);
        Assert.Null(response);
    }
    #endregion

}
