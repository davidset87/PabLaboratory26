using AppCore.Mapper;
using AppCore.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppCore.Modules;

public static class ContactsModule
{
    public static IServiceCollection AddContactsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        
        services.AddValidatorsFromAssemblyContaining<CreatePersonDtoValidator>();
       
        services.AddAutoMapper(typeof(ContactsMappingProfile).Assembly);
        
        return services;
    }
}