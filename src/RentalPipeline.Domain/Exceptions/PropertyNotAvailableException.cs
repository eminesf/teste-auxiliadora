namespace RentalPipeline.Domain.Exceptions;

public class PropertyNotAvailableException : Exception
{
    public PropertyNotAvailableException(Guid propertyId)
        : base($"O imóvel '{propertyId}' não está disponível para novas propostas.")
    { }
}