using RentalPipeline.Domain.Enums;

namespace RentalPipeline.Domain.Entities;

public class Property
{
    public Guid Id { get; private set; }
    public Guid OwnerId { get; private set; }
    public Client Owner { get; private set; } = null!;
    public string Street { get; private set; } = string.Empty;
    public string Number { get; private set; } = string.Empty;
    public string? Complement { get; private set; }
    public string District { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public decimal RentPrice { get; private set; }
    public PropertyStatus Status { get; set; }
    public DateTime CreatedAt { get; private set; }

    protected Property() { }

    public Property(Guid ownerId, string street, string number, string district, string city, string state, decimal rentPrice, string? complement = null)
    {
        Id = Guid.NewGuid();
        OwnerId = ownerId;
        Street = street;
        Number = number;
        Complement = complement;
        District = district;
        City = city;
        State = state;
        RentPrice = rentPrice;
        Status = PropertyStatus.Available;
        CreatedAt = DateTime.UtcNow;
    }
}