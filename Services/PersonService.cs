using System.Globalization;
using CsvHelper;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using CsvHelper.Configuration;
using OfficeOpenXml;

namespace Services;

public class PersonService : IPersonService
{
    private readonly PersonsDbContext _db;

    public PersonService(PersonsDbContext personsDbContext)
    {
        _db = personsDbContext;
    }


    public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
    {
        // check if PersonAddRequest is not null
        ArgumentNullException.ThrowIfNull(request);

        // Model Validations
        ValidationHelpers.ModelValidation(request);
        // ValidationContext validationContext = new ValidationContext(request);
        // List<ValidationResult> validationResults = new List<ValidationResult>();
        //
        //
        // bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);
        // if (!isValid)
        // {
        //     throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
        // }

        // Validate Name
        // if (string.IsNullOrWhiteSpace(request.Name))
        // {
        //     throw new ArgumentNullException(nameof(request));
        // }

        // Convert personAddRequest into Person type
        Person person = request.ToPerson();


        // Generate PersonId
        person.PersonId = Guid.NewGuid();

        await _db.Persons.AddAsync(person);
        await _db.SaveChangesAsync();

        // Convert the person object into PersonResponse type
        return person.ToPersonResponse();
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        var _persons = await _db.Persons.Include("Country").ToListAsync();
        return [.. _persons.Select(person => person.ToPersonResponse())];
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
    {
        List<PersonResponse> allPersons = await GetAllPersons();
        List<PersonResponse> matchingPersons = allPersons;


        if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty((searchString))) return matchingPersons;

        matchingPersons = searchBy switch
        {
            nameof(PersonResponse.Name) => [.. allPersons.Where(temp =>
                    temp.Name?.Contains(searchString,
                        StringComparison.OrdinalIgnoreCase) == true
                )],
            nameof(PersonResponse.Email) => [.. allPersons.Where(temp =>
                    temp.Email?.Contains(searchString,
                        StringComparison.OrdinalIgnoreCase) == true
                )],
            nameof(Person.DateOfBirth) => [.. allPersons.Where(temp =>
                    temp.DateOfBirth?.ToString("dd MMMM yyyy")
                        .Contains(searchString,
                            StringComparison.OrdinalIgnoreCase) == true
                )],
            nameof(Person.Gender) => [.. allPersons.Where(temp =>
                    temp.Gender?.Contains(searchString,
                        StringComparison.OrdinalIgnoreCase) == true
                )],
            nameof(Person.CountryId) => [.. allPersons.Where(temp =>
                    temp.Country?.Contains(searchString,
                        StringComparison.OrdinalIgnoreCase) == true
                )],
            nameof(Person.Address) => [.. allPersons.Where(temp =>
                    temp.Address?.Contains(searchString,
                        StringComparison.OrdinalIgnoreCase) == true
                )],
            _ => allPersons,
        };
        return matchingPersons;
    }

    public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderEnum sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy))
        {
            return allPersons;
        }

        List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
        {
            (nameof(PersonResponse.Name), SortOrderEnum.Ascending) =>
                [.. allPersons.OrderBy(temp => temp.Name, StringComparer.OrdinalIgnoreCase)],

            (nameof(PersonResponse.Name), SortOrderEnum.Descending) =>
                [.. allPersons.OrderByDescending(temp => temp.Name, StringComparer.OrdinalIgnoreCase)],

            (nameof(PersonResponse.Email), SortOrderEnum.Ascending) =>
    [.. allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase)],

            (nameof(PersonResponse.Email), SortOrderEnum.Descending) =>
                [.. allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase)],


            (nameof(PersonResponse.DateOfBirth), SortOrderEnum.Ascending) =>
                [.. allPersons.OrderBy(temp => temp.DateOfBirth)],

            (nameof(PersonResponse.DateOfBirth), SortOrderEnum.Descending) =>
                [.. allPersons.OrderByDescending(temp => temp.DateOfBirth)],



            (nameof(PersonResponse.Age), SortOrderEnum.Ascending) =>
                [.. allPersons.OrderBy(temp => temp.Age)],

            (nameof(PersonResponse.Age), SortOrderEnum.Descending) =>
                [.. allPersons.OrderByDescending(temp => temp.Age)],


            (nameof(PersonResponse.Gender), SortOrderEnum.Ascending) =>
                [.. allPersons.OrderBy(temp => temp.Gender)],

            (nameof(PersonResponse.Gender), SortOrderEnum.Descending) =>
                [.. allPersons.OrderByDescending(temp => temp.Gender)],

            (nameof(PersonResponse.Country), SortOrderEnum.Ascending) =>
                [.. allPersons.OrderBy(temp => temp.Country)],

            (nameof(PersonResponse.Country), SortOrderEnum.Descending) =>
                [.. allPersons.OrderByDescending(temp => temp.Country)],


            (nameof(PersonResponse.Address), SortOrderEnum.Descending) =>
                [.. allPersons.OrderByDescending(temp => temp.Address)],


            (nameof(PersonResponse.ReceiveNewsLetter), SortOrderEnum.Ascending) =>
                [.. allPersons.OrderByDescending(temp => temp.ReceiveNewsLetter)],


            (nameof(PersonResponse.ReceiveNewsLetter), SortOrderEnum.Descending) =>
                [.. allPersons.OrderByDescending(temp => temp.ReceiveNewsLetter)],

            _ => allPersons



        };

        return sortedPersons;

    }

    public async Task<PersonResponse?> UpdatePerson(PersonUpdateRequest? request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Name == null) throw new ArgumentException(nameof(request.Name));


        // Validation
        ValidationHelpers.ModelValidation(request);

        // get matching person to update
        var matchingPerson = await _db.Persons.FirstOrDefaultAsync(person => person.PersonId == request.PersonId);
        if (matchingPerson != null)
        {
            // update all the details
            matchingPerson.Name = request.Name;
            matchingPerson.Email = request.Email;
            matchingPerson.DateOfBirth = request.DateOfBirth;
            matchingPerson.Gender = request.Gender.ToString();
            matchingPerson.CountryId = request.CountryId;
            matchingPerson.Address = request.Address;
            matchingPerson.ReceiveNewsLetter = request.ReceiveNewsLetter;

            _db.Persons.Update(matchingPerson);
            await _db.SaveChangesAsync();
            return matchingPerson.ToPersonResponse();
        }

        throw new ArgumentException("Given person id does not exist");
    }

    public async Task<bool> DeletePerson(Guid? personId)
    {
        if (personId == null) throw new ArgumentNullException(nameof(personId));
        var person = await _db.Persons.FirstOrDefaultAsync(person => person.PersonId == personId);
        if (person == null) return false;
        _db.Persons.Remove(person);
        _db.SaveChanges();
        return true;
    }

    public async Task<PersonResponse?> GetPersonByPersonId(Guid? personId)
    {
        if (personId == null) return null;
        Person? person = await _db.Persons.FirstOrDefaultAsync(person => person.PersonId == personId);
        if (person == null) return null;
        return person.ToPersonResponse();
    }

    public async Task<MemoryStream> GetPersonsCSV()
    {
        // MemoryStream memoryStream = new();
        // StreamWriter streamWriter = new(memoryStream);
        // var persons = await _db.Persons.Select(temp => temp.ToPersonResponse()).ToListAsync();
        // CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture);
        //
        //
        // CsvWriter csvWriter = new(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true);
        //
        // // Write CSV Headers
        // csvWriter.WriteHeader<PersonResponse>();
        // await csvWriter.NextRecordAsync();
        // // Write the body to the csv
        // await csvWriter.WriteRecordsAsync(persons);
        // await csvWriter.FlushAsync();
        // await streamWriter.FlushAsync();
        // memoryStream.Position = 0;
        // return memoryStream;



        MemoryStream memoryStream = new();
        StreamWriter streamWriter = new(memoryStream);
        var persons = await _db.Persons.Include("Country").Select(temp => temp.ToPersonResponse()).ToListAsync();
        CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture);


        CsvWriter csvWriter = new(streamWriter, csvConfiguration);

        // Write CSV Headers
        csvWriter.WriteField(nameof(PersonResponse.Name));
        csvWriter.WriteField(nameof(PersonResponse.Email));
        csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
        csvWriter.WriteField(nameof(PersonResponse.Age));
        csvWriter.WriteField(nameof(PersonResponse.Gender));
        csvWriter.WriteField(nameof(PersonResponse.Country));
        csvWriter.WriteField(nameof(PersonResponse.Address));
        csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetter));

        await csvWriter.NextRecordAsync();
        await csvWriter.FlushAsync();

        foreach (var person in persons)
        {
            csvWriter.WriteField(person.Name);
            csvWriter.WriteField(person.Email);
            if (person.DateOfBirth.HasValue)
                csvWriter.WriteField(person.DateOfBirth);
            else
                csvWriter.WriteField("");
            csvWriter.WriteField(person.Age);
            csvWriter.WriteField(person.Gender);
            csvWriter.WriteField(person.Country);
            csvWriter.WriteField(person.Address);
            csvWriter.WriteField(person.ReceiveNewsLetter);
            await csvWriter.NextRecordAsync();
            await csvWriter.FlushAsync();
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task<MemoryStream> GetPersonsExcel()
    {
        MemoryStream memoryStream = new();
        using (var package = new ExcelPackage(memoryStream))
        {
            var sheet = package.Workbook.Worksheets.Add("PersonsSheet");
            sheet.Cells["A1"].Value = "Name";
            sheet.Cells["B1"].Value = "Email";
            sheet.Cells["C1"].Value = "Date Of Birth";
            sheet.Cells["D1"].Value = "Age";
            sheet.Cells["E1"].Value = "Gender";
            sheet.Cells["F1"].Value = "Country";
            sheet.Cells["G1"].Value = "Address";
            sheet.Cells["H1"].Value = "Receive News Letter";

            int row = 2;

            var persons = await _db.Persons.Include("Country").Select(temp => temp.ToPersonResponse()).ToListAsync();

            foreach (var person in persons)
            {
                sheet.Cells[row, 1].Value = person.Name;
                sheet.Cells[row, 2].Value = person.Email;
                if (person.DateOfBirth.HasValue)
                    sheet.Cells[row, 3].Value = person.DateOfBirth.Value.ToString("yyyy-MM-dd");
                sheet.Cells[row, 4].Value = person.Age;
                sheet.Cells[row, 5].Value = person.Gender;
                sheet.Cells[row, 6].Value = person.Country;
                sheet.Cells[row, 7].Value = person.Address;
                sheet.Cells[row, 8].Value = person.ReceiveNewsLetter;

                row++;
            }
            sheet.Cells[$"A1:H{row}"].AutoFitColumns();
            await package.SaveAsync();

        }
        memoryStream.Position = 0;
        return memoryStream;
    }
}
