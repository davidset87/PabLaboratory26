namespace AppCore.Dto;

public record RefreshTokenDto(
    string AccessToken,
    string RefreshToken
);