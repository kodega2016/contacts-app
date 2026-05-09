using Entities;

namespace ServiceContracts.DTO;

/// <summary>
/// Represents DTO class that is used to return the response from the Persons Service
/// </summary>
public class PersonResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string?Email { get; set; }
    public DateTime?DateOfBirth { get; set; }
    public string?Gender { get; set; }
    public string? Country { get; set; }
    public string?Address { get; set; }
    public int? Age { get; set; }
    public bool ReceiveNewsLetter { get; set; }

    protected bool Equals(PersonResponse other)
    {
        return Id.Equals(other.Id) && Name == other.Name && Email == other.Email && Nullable.Equals(DateOfBirth, other.DateOfBirth) && Gender == other.Gender && Nullable.Equals(Country, other.Country) && Address == other.Address && Age == other.Age && ReceiveNewsLetter == other.ReceiveNewsLetter;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((PersonResponse)obj);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Id);
        hashCode.Add(Name);
        hashCode.Add(Email);
        hashCode.Add(DateOfBirth);
        hashCode.Add(Gender);
        hashCode.Add(Country);
        hashCode.Add(Address);
        hashCode.Add(Age);
        hashCode.Add(ReceiveNewsLetter);
        return hashCode.ToHashCode();
    }
}

public static class PersonResponseExtensions
{
    public static PersonResponse ToPersonResponse(this Person person)
    {
        return new PersonResponse()
        {
            Id = person.PersonId,
            Name = person.Name,
            Email = person.Email,
            DateOfBirth = person.DateOfBirth,
            Gender = person.Gender,
            Address = person.Address,
            ReceiveNewsLetter = person.ReceiveNewsLetter,
            Age = person.DateOfBirth != null
                ? (int?)Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25)
                : null,
        };
    }
}