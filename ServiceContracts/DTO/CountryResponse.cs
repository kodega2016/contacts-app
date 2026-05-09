using Entities;

namespace ServiceContracts.DTO;

/// <summary>
/// DTO class that is used as return type for most of Countries Service methods
/// </summary>
public class CountryResponse
{
    public Guid CountryId { get; set; }
    public string?CountryName { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj.GetType() != GetType()) return false;
        CountryResponse countryToCompare=(CountryResponse)obj;
        return CountryId.Equals(countryToCompare.CountryId)
               && CountryName != null
               && CountryName.Equals(countryToCompare.CountryName);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CountryId, CountryName);
    }
}

public static class CountryExtensions
{
    public static CountryResponse ToCountryResponse(this Country country)
    {
        return new CountryResponse
        {
            CountryId = country.CountryID,
            CountryName = country.CountryName
        };
    }

   
}