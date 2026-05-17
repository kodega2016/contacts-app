using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts;

/// <summary>
/// Represents business logic for manipulating country entry
/// </summary>
public interface ICountriesService
{
    /// <summary>
    /// Adds a country object to the list of countries
    /// </summary>
    /// <param name="countryAddRequest">County object to add</param>
    /// <returns>Returns the country object after adding it(including newly generated id)</returns>
    Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<List<CountryResponse>> GetCountries();

    /// <summary>
    /// Returns a country object based on the given country id
    /// </summary>
    /// <param name="countryId"></param>
    /// <returns>return a matching CountryResponse object</returns>

    Task<CountryResponse?> GetCountryByCountryId(Guid? countryId);


  Task<int>  UploadCountriesFromExcelFile(IFormFile file);
}


