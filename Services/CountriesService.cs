using System.Security.Cryptography.X509Certificates;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService:ICountriesService
{
    private readonly List<Country> _countries;

    public CountriesService()
    {
        _countries = new List<Country>();
    }
    public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
    {
        // Validation: countryAddRequest parameter cannot be null
        if (countryAddRequest == null) throw new ArgumentNullException(nameof(countryAddRequest));
        
        // Validation: CountryName cannot be null
        if(countryAddRequest.CountryName==null) throw new ArgumentNullException(nameof(countryAddRequest.CountryName));
        
        // Validation: CountryName cannot be duplicate
        if (_countries.Any(temp => temp.CountryName == countryAddRequest.CountryName))
        {
            throw new ArgumentException("Given country name already exists");
        }
        
        // Convert object from CountryAddRequest to Country type
        Country country = countryAddRequest.ToCountry();
        
        // Generate CountryID
        country.CountryID = Guid.NewGuid();
        
        // Add country to the list of countries
        _countries.Add(country);

        return country.ToCountryResponse();
    }

    public List<CountryResponse> GetCountries()
    {
        return _countries.Select(country=>country.ToCountryResponse()).ToList();
    }

    public CountryResponse? GetCountryByCountryId(Guid? countryId)
    {
        if (countryId == null) return null;
        var country = _countries.FirstOrDefault(temp => temp.CountryID == countryId);
        return country?.ToCountryResponse();
    }


  
}
