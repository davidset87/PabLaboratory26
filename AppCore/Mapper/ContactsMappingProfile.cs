using AppCore.Dto;
using AppCore.Models;
using AutoMapper;

namespace AppCore.Mapper;

public class ContactsMappingProfile : Profile
{
    public ContactsMappingProfile()
    {
        
        CreateMap<Person, PersonDto>()
            .ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(
                dest => dest.EmployerName,
                opt => opt.MapFrom(src => src.Employer != null ? src.Employer.Name : null));

        // Mapowanie Address <-> AddressDto (dwukierunkowe)
        CreateMap<Address, AddressDto>().ReverseMap();
        
        // Mapowanie CreatePersonDto -> Person
        CreateMap<CreatePersonDto, Person>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Notes, opt => opt.Ignore())
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
        
        // Mapowanie UpdatePersonDto -> Person
        CreateMap<UpdatePersonDto, Person>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Notes, opt => opt.Ignore())
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}