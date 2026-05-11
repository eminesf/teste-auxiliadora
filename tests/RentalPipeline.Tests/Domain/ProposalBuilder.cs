using RentalPipeline.Domain.Entities;
using RentalPipeline.Domain.Enums;

namespace RentalPipeline.Tests.Domain;

public static class ProposalBuilder
{
  public static (Proposal proposal, Property property) Build()
  {
    var owner = new Client("Owner Silva", "owner@email.com", "111.111.111-11", "senha123", "Owner");
    var property = new Property(
        owner.Id, "Rua das Flores", "123", "Centro", "Porto Alegre", "RS", 1500m);

    var propertyProp = typeof(Proposal)
        .GetProperty("Property",
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance);

    var proposal = new Proposal(property.Id, owner.Id);

    propertyProp!.SetValue(proposal, property);

    return (proposal, property);
  }
}