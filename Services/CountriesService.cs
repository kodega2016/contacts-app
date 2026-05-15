using System.Security.Cryptography.X509Certificates;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService : ICountriesService
{
    private readonly PersonsDbContext _db;

    public CountriesService(PersonsDbContext personsDbContext)
    {
        _db = personsDbContext;
    }
    public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
    {
        // Validation: countryAddRequest parameter cannot be null
        if (countryAddRequest == null) throw new ArgumentNullException(nameof(countryAddRequest));

        // Validation: CountryName cannot be null
        if (countryAddRequest.CountryName == null) throw new ArgumentNullException(nameof(countryAddRequest.CountryName));

        // Validation: CountryName cannot be duplicate
        if (_db.Countries.Any(temp => temp.CountryName == countryAddRequest.CountryName))
        {
            throw new ArgumentException("Given country name already exists");
        }

        // Convert object from CountryAddRequest to Country type
        Country country = countryAddRequest.ToCountry();

        // Generate CountryID
        country.CountryID = Guid.NewGuid();

        // Add country to the list of countries
        _db.Countries.Add(country);
        _db.SaveChanges();

        return country.ToCountryResponse();
    }

    public List<CountryResponse> GetCountries()
    {
        var _countries=_db.Countries.Include("Persons").ToList();
        return _countries.Select(country => country.ToCountryResponse()).ToList();
    }

    public CountryResponse? GetCountryByCountryId(Guid? countryId)
    {
        if (countryId == null) return null;
        var country = _db.Countries.FirstOrDefault(temp => temp.CountryID == countryId);
        return country?.ToCountryResponse();
    }



}
