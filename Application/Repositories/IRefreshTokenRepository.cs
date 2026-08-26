using Application.Models;

namespace Application.Repositories;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetRefreshToken(int userId);
    Task<RefreshToken?> GetRefreshToken(string refreshToken);
    Task<RefreshToken> UpsertToken(RefreshToken refreshToken);
}
