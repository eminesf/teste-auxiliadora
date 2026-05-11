using FluentValidation;
using RentalPipeline.Application.DTOs.Requests;

namespace RentalPipeline.Application.Validators;

public class CreateProposalValidator : AbstractValidator<CreateProposalRequest>
{
  public CreateProposalValidator()
  {
    RuleFor(x => x.PropertyId)
        .NotEmpty().WithMessage("PropertyId é obrigatório.");

    RuleFor(x => x.ClientId)
        .NotEmpty().WithMessage("ClientId é obrigatório.");
  }
}