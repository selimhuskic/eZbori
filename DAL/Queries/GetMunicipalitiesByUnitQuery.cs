using DAL.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DAL.Queries;

public record GetMunicipalitiesByUnitQuery(int Code) : IRequest<IEnumerable<string>>;

public sealed class GetMunicipalitiesByUnitQueryHandler(eZboriDbContext dbContext)
    : IRequestHandler<GetMunicipalitiesByUnitQuery, IEnumerable<string>>
{
    public async Task<IEnumerable<string>> Handle(
        GetMunicipalitiesByUnitQuery request, CancellationToken ct)
    {
        // 511–523 = state parliament units; 301–412 = entity parliament units
        if (request.Code is >= 511 and <= 523)
            return await dbContext.Municipalities
                .Where(m => (int)m.StateParliamentElectoralUnit == request.Code)
                .Select(m => m.Name)
                .OrderBy(m => m)
                .ToListAsync(ct);

        return await dbContext.Municipalities
            .Where(m => (int)m.EntityParliamentElectoralUnit == request.Code)
            .Select(m => m.Name)
            .OrderBy(m => m)
            .ToListAsync(ct);
    }
}
