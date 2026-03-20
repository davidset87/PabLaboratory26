using AppCore.Enums;

namespace AppCore.Dto;

public record ContactSearchDto(
    string? Query,
    ContactStatus? Status,
    string? Tag,
    string? ContactType,
    int Page = 1,
    int PageSize = 20
);