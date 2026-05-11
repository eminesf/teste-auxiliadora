using FluentValidation;
using RentalPipeline.Application.DTOs.Requests;

namespace RentalPipeline.Application.Validators;

public class CreatePropertyValidator : AbstractValidator<CreatePropertyRequest>
{
  public CreatePropertyValidator()
  {
    RuleFor(x => x.OwnerId)
        .NotEmpty().WithMessage("OwnerId é obrigatório.");

    RuleFor(x => x.Street)
        .NotEmpty().WithMessage("Rua é obrigatória.")
        .MaximumLength(300).WithMessage("Rua deve ter no máximo 300 caracteres.");

    RuleFor(x => x.Number)
        .NotEmpty().WithMessage("Número é obrigatório.")
        .MaximumLength(20).WithMessage("Número deve ter no máximo 20 caracteres.");

    RuleFor(x => x.Complement)
        .MaximumLength(200).WithMessage("Complemento deve ter no máximo 200 caracteres.")
        .When(x => x.Complement is not null);

    RuleFor(x => x.District)
        .NotEmpty().WithMessage("Bairro é obrigatório.")
        .MaximumLength(200).WithMessage("Bairro deve ter no máximo 200 caracteres.");

    RuleFor(x => x.City)
        .NotEmpty().WithMessage("Cidade é obrigatória.")
        .MaximumLength(200).WithMessage("Cidade deve ter no máximo 200 caracteres.");

    RuleFor(x => x.State)
        .NotEmpty().WithMessage("Estado é obrigatório.")
        .Length(2).WithMessage("Estado deve ter exatamente 2 caracteres (ex: RS).");

    RuleFor(x => x.RentPrice)
        .GreaterThan(0).WithMessage("Valor do aluguel deve ser maior que zero.");
  }
}