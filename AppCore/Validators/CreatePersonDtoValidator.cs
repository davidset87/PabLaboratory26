using AppCore.Dto;
using AppCore.Repositories;
using FluentValidation;

namespace AppCore.Validators;

public class CreatePersonDtoValidator : AbstractValidator<CreatePersonDto>
{
    private readonly ICompanyRepository _companyRepository;
    
    public CreatePersonDtoValidator(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
        
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Imię jest wymagane.")
            .MaximumLength(100).WithMessage("Imię nie może przekraczać 100 znaków.")
            .Matches(@"^[\p{L}\s\-]+$").WithMessage("Imię zawiera niedozwolone znaki.");

        // Reguła dla LastName
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Nazwisko jest wymagane.")
            .MaximumLength(200).WithMessage("Nazwisko nie może przekraczać 200 znaków.")
            .Matches(@"^[\p{L}\s\-]+$").WithMessage("Nazwisko zawiera niedozwolone znaki.");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email jest wymagany.")
            .EmailAddress().WithMessage("Nieprawidłowy format adresu email.")
            .MaximumLength(200).WithMessage("Email nie może przekraczać 200 znaków.");

        // Format numeru telefonu - akceptuje różne formaty
        RuleFor(x => x.Phone)
            .Matches(@"^(\+?\d{1,3}[-\s]?)?\d{2,3}[-\s]?\d{3}[-\s]?\d{2,3}$")
                .WithMessage("Nieprawidłowy format numeru telefonu. Oczekiwane formaty: +48 123 456 789, 123-456-789, 123456789")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Today.AddYears(-18))
                .WithMessage("Osoba musi mieć co najmniej 18 lat.")
            .GreaterThan(DateTime.Today.AddYears(-120))
                .WithMessage("Nieprawidłowa data urodzenia.")
            .When(x => x.BirthDate.HasValue);

        RuleFor(x => x.Gender)
            .IsInEnum()
                .WithMessage("Nieprawidłowa wartość płci.");

        RuleFor(x => x.EmployerId)
            .MustAsync(EmployerExistsAsync)
                .WithMessage("Wskazana firma nie istnieje.")
            .When(x => x.EmployerId.HasValue);

        RuleFor(x => x.Address)
            .SetValidator(new AddressDtoValidator()!)
            .When(x => x.Address is not null);
    }

    private async Task<bool> EmployerExistsAsync(Guid? employerId, CancellationToken ct)
    {
        if (employerId == null) return true;
        var company = await _companyRepository.FindByIdAsync(employerId.Value);
        return company != null;
    }
}