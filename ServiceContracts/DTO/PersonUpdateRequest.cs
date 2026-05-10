using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;

/// <summary>
/// Represents the DTO class that contains the person details to
/// update
/// </summary>
public class PersonUpdateRequest
{
    [Required(ErrorMessage = "Person Id cannot be blank")]
    public Guid PersonId { get; set; }

    [Required(ErrorMessage = "Name cannot be blank")]
    public string? Name { get; set; }

    [Required] [EmailAddress] public string? Email { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public GenderOptions? Gender { get; set; }

    public Guid? CountryId { get; set; }

    public string? Address { get; set; }

    public bool ReceiveNewsLetter { get; set; }
    
    /// <summary>
    /// Converts the current object of PersonUpdateRequest into a new object ot Person type
    /// </summary>
    /// <returns></returns>

    public Person ToPerson()
    {
        return new Person()
        {
            PersonId = PersonId,
            Name = Name,
            Email = Email,
            DateOfBirth = DateOfBirth,
            Gender = Gender.ToString(),
            CountryId = CountryId,
            ReceiveNewsLetter = ReceiveNewsLetter,
        };
    }
}