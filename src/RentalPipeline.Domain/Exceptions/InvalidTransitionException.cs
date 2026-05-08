namespace RentalPipeline.Domain.Exceptions;

public class InvalidTransitionException : Exception
{
    public InvalidTransitionException(string from, string to, IEnumerable<string> allowed)
        : base($"Transição inválida: '{from}' → '{to}'. " +
               $"Transições permitidas a partir deste estado: {string.Join(", ", allowed)}")
    { }
}