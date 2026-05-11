using FluentValidation;
using RentalPipeline.Application.DTOs.Requests;

namespace RentalPipeline.Application.Validators;

public class CreateClientValidator : AbstractValidator<CreateClientRequest>
{
  public CreateClientValidator()
  {
    RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Nome é obrigatório.")
        .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres.");

    RuleFor(x => x.Email)
        .NotEmpty().WithMessage("Email é obrigatório.")
        .EmailAddress().WithMessage("Email inválido.")
        .MaximumLength(200).WithMessage("Email deve ter no máximo 200 caracteres.");

    RuleFor(x => x.Document)
        .NotEmpty().WithMessage("Documento é obrigatório.")
        .MaximumLength(14).WithMessage("Documento deve ter no máximo 14 caracteres.");
  }
}