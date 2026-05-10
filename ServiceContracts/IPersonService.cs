using ServiceContracts.DTO;

namespace ServiceContracts;

/// <summary>
/// Represents business logic for manipulating Person entity
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Adds a new person to the list of Person
    /// </summary>
    /// <param name="request">Person to add</param>
    /// <returns>Returns the same person detail,along with newly generated PersonId</returns>
    PersonResponse AddPerson(PersonAddRequest? request);

    /// <summary>
    /// Returns all person
    /// </summary>
    /// <returns>Return a list of PersonResponse Object</returns>
    List<PersonResponse> GetAllPersons();

    /// <summary>
    /// Return an object of PersonResponse from the list of persons
    /// Where id matches the passed id
    /// </summary>
    /// <param name="personId"></param>
    /// <returns>Return an object of Person Response</returns>
    PersonResponse? GetPersonByPersonId(Guid? personId);
    
    /// <summary>
    /// Returns all person objects that matches with the give
    /// search field and search string
    /// </summary>
    /// <param name="searchBy">Search field to search</param>
    /// <param name="searchString">Search string to search</param>
    /// <returns>Returns all matching persons based on the given search field and search string</returns>

    List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString);
}
