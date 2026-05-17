using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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
    public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
    {
        // Validation: countryAddRequest parameter cannot be null
        ArgumentNullException.ThrowIfNull(countryAddRequest);

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
        await _db.Countries.AddAsync(country);
        await _db.SaveChangesAsync();
        return country.ToCountryResponse();
    }

    public async Task<List<CountryResponse>> GetCountries()
    {
        var _countries = _db.Countries.Include("Persons").ToList();
        return _countries.Select(country => country.ToCountryResponse()).ToList();
    }

    public async Task<CountryResponse?> GetCountryByCountryId(Guid? countryId)
    {
        if (countryId == null) return null;
        var country = await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryID == countryId);
        return country?.ToCountryResponse();
    }

    public async Task<int> UploadCountriesFromExcelFile(IFormFile file)
    {
        MemoryStream memoryStream = new();
        await file.CopyToAsync(memoryStream);
        var countriesInserted = 0;
        using (ExcelPackage excelPackage = new(memoryStream))
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Countries"];
            Console.WriteLine($"Worksheet:{worksheet}");
            int rowCount = worksheet.Dimension.Rows;
              
            for (int row = 2; row <= rowCount; row++)
            {
                string? cellValue = Convert.ToString(worksheet.Cells[row, 1].Value);
                if (!string.IsNullOrEmpty(cellValue))
                {
                    string? countryName = cellValue;
                    if (!_db.Countries.Where(temp => temp.CountryName == countryName).Any())
                    {
                        var country = new Country()
                        {
                            CountryName = countryName
                        };

                        await _db.Countries.AddAsync(country);
                        await _db.SaveChangesAsync();
                        countriesInserted++;
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ duplicate entry:{countryName}");
                    }
                }
            }

        }
        return countriesInserted;
    }
}
