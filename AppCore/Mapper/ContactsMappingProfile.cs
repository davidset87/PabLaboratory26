using AppCore.Dto;
using AppCore.Enums;
using AppCore.Models;
using AppCore.ValueObjects;
using AutoMapper;

namespace AppCore.Mapper;

public class ContactsMappingProfile : Profile
{
    public ContactsMappingProfile()
    {
        // Map Person -> PersonDto
        CreateMap<Person, PersonDto>()
            .ForMember(
                dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(
                dest => dest.EmployerName,
                opt => opt.MapFrom(src => src.Employer != null ? src.Employer.Name : null))
            .ForMember(
                dest => dest.Address,
                opt => opt.MapFrom(src => src.Address))
            .ForMember(
                dest => dest.Notes,
                opt => opt.MapFrom(src => src.Notes));

        // Map Address <-> AddressDto (both ways)
        CreateMap<Address, AddressDto>()
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name))
            .ReverseMap()
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => new Country { Name = src.Country, Code = src.Country }));

        // Map Note -> NoteDto
        CreateMap<Note, NoteDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.ContactId, opt => opt.MapFrom(src => src.ContactId));

        // Map CreatePersonDto -> Person
        CreateMap<CreatePersonDto, Person>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.Notes, opt => opt.Ignore())
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ContactStatus.Active));

        // Map UpdatePersonDto -> Person
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