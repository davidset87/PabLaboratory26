using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security;

public class JwtSettings
{
    private readonly IConfiguration _configuration;
    private static readonly string Section = "Jwt";

    public JwtSettings(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Issuer => _configuration.GetSection(Section)["Issuer"] 
                            ?? throw new InvalidOperationException("Issuer is not set.");

    public string Audience => _configuration.GetSection(Section)["Audience"] 
                              ?? throw new InvalidOperationException("Audience is not set.");

    public string Secret => _configuration.GetSection(Section)["SecretKey"] 
                            ?? throw new InvalidOperationException("Secret key is not set.");

    public int ExpirationInMinutes => _configuration.GetSection(Section).GetValue<int>("ExpiryInMinutes");

    public int RefreshTokenDays => _configuration.GetSection(Section).GetValue<int>("RefreshTokenDays");

    public SymmetricSecurityKey GetSymmetricKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
}