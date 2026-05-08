namespace RentalPipeline.Domain.Entities;

public class Client
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Document { get; private set; } = string.Empty; // CPF
    public DateTime CreatedAt { get; private set; }

    protected Client() { }

    public Client(string name, string email, string document)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Document = document;
        CreatedAt = DateTime.UtcNow;
    }
}