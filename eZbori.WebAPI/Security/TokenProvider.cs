using Application.Repositories;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace eZbori.Web.Security;

public sealed class TokenProvider(
    IConfiguration configuration,
    IRefreshTokenRepository refreshTokenRepository)
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    public string GenerateJwtToken(User user)
    {
        var secretKey = _configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Birthdate, user.DateOfBirth.ToString()),
                new Claim(JwtRegisteredClaimNames.EmailVerified, user.UserVerified.ToString()),
                new Claim(ClaimTypes.Role, user.Role.RoleName)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
            SigningCredentials = credentials,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var handler = new JsonWebTokenHandler();

        return handler.CreateToken(descriptor);
    }

    public async Task<string> GenerateRefreshToken(User user)
    {
        var refreshToken = Guid.NewGuid().ToString();
        var expiry = DateTime.UtcNow.AddDays(7);

        var token = await _refreshTokenRepository.GetRefreshToken(user.Id);

        token = token is not null ?
            token.UpdateToken(refreshToken, DateTime.UtcNow, expiry) :
            new RefreshToken(user.Id, refreshToken, DateTime.UtcNow, expiry);

        await _refreshTokenRepository.UpsertToken(token);

        return refreshToken;
    }

    public async Task<User?> ValidateRefreshToken(string refreshToken)
    {
        var tokenEntry = await _refreshTokenRepository.GetRefreshToken(refreshToken);

        return tokenEntry?.User;
    }

    public async Task<RefreshToken?> GetToken(int userId) =>
        await _refreshTokenRepository.GetRefreshToken(userId);

    public async Task RemoveToken(RefreshToken refreshToken) =>
        await _refreshTokenRepository.DeleteAsync(refreshToken);
}
