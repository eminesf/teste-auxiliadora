namespace RentalPipeline.Domain.Entities;

public class Client
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Document { get; private set; } = string.Empty; // CPF
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    protected Client() { }

    public Client(string name, string email, string document, string password, string role)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Document = CleanDocument(document);
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }

    public bool VerifyPassword(string password) =>
            BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    private static string CleanDocument(string document) =>
        new string(document.Where(char.IsDigit).ToArray());
}