using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO;
/// <summary>
/// Acts as a DTO for inserting a new person
/// </summary>
public class PersonAddRequest
{
    [Required(ErrorMessage = "Name cannot be blank")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Email value should be a valid email")]
    [EmailAddress(ErrorMessage = "Email value should be a valid email")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime? DateOfBirth { get; set; }


    [Required]
    public GenderOptions? Gender { get; set; }


    [Required]
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }

    public bool ReceiveNewsLetter { get; set; }


    /// <summary>
    /// Converts the current object of PersonAddRequest into a new object ot Person type
    /// </summary>
    /// <returns></returns>
    public Person ToPerson()
    {
        return new Person()
        {
            Name = Name,
            Email = Email,
            DateOfBirth = DateOfBirth,
            Gender = Gender.ToString(),
            CountryId = CountryId,
            ReceiveNewsLetter = ReceiveNewsLetter,
        };
    }

}
