using AppCore.Dto;
using FluentValidation;

namespace AppCore.Validators;

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Ulica jest wymagana.")
            .MaximumLength(200).WithMessage("Ulica nie może przekraczać 200 znaków.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("Miasto jest wymagane.")
            .MaximumLength(100).WithMessage("Miasto nie może przekraczać 100 znaków.");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Kod pocztowy jest wymagany.")
            .Matches(@"^\d{2}-\d{3}$").WithMessage("Nieprawidłowy format kodu pocztowego. Oczekiwany format: XX-XXX");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Kraj jest wymagany.")
            .MaximumLength(100).WithMessage("Kraj nie może przekraczać 100 znaków.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Nieprawidłowy typ adresu.");
    }
}