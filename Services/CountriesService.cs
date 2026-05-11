using System.Security.Cryptography.X509Certificates;
using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services;

public class CountriesService:ICountriesService
{
    private readonly List<Country> _countries;

    public CountriesService(bool initialize=true)
    {
        _countries = new List<Country>();
        if (initialize)
        {
            _countries.AddRange(new List<Country>()
            {
                new Country()
                {
                    CountryID = Guid.Parse("970909DE-5FDB-4DEE-91C6-B5945C5525EC"),
                    CountryName = "United Kingdom",
                },
                new Country()
                {
                    CountryID = Guid.Parse("3D1E7784-F89D-4D73-9BE3-2756C619D876"),
                    CountryName = "United States",
                },
                new Country()
                {
                    CountryID = Guid.Parse("AF90F37D-DFB4-4514-AB16-3A088A2994C7"),
                    CountryName = "Nepal",
                }
            });
        }
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
