using FluentValidation;
using RentalPipeline.Application.DTOs.Requests;
using RentalPipeline.Domain.Enums;

namespace RentalPipeline.Application.Validators;

public class TransitionProposalValidator : AbstractValidator<TransitionProposalRequest>
{
  public TransitionProposalValidator()
  {
    RuleFor(x => x.NewStatus)
        .NotEmpty().WithMessage("NewStatus é obrigatório.")
        .Must(BeAValidStatus).WithMessage(
            $"Status inválido. Valores aceitos: {string.Join(", ", Enum.GetNames<ProposalStatus>())}");
  }

  private static bool BeAValidStatus(string status) =>
      Enum.TryParse<ProposalStatus>(status, ignoreCase: true, out _);
}