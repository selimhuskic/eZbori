using Application.DTOs;
using Application.Enum;
using Application.Interfaces;
using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DAL.Analysis.Strategies;

public sealed class EntityMunicipalPartiesStrategy(eZboriDbContext dbContext) : IPartiesResultsStrategy
{
    public AnalysisSubject Subject => AnalysisSubject.EntityMunicipalParties;

    public async Task<IEnumerable<PartiesResults>> GetAsync(AnalysisRequest request, CancellationToken ct)
        => await (from o in dbContext.EntityMunicipalParty
                  join m in dbContext.Municipalities on o.MunicipalityCode equals m.Id
                  where o.ElectionYear == request.SelectedYear &&
                        (request.MunicipalityCode == null || o.MunicipalityCode == request.MunicipalityCode)
                  orderby o.TotalVotes descending
                  select new PartiesResults(
                      o.MunicipalityCode,
                      m.Name,
                      o.ElectionYear,
                      ElectionType.GeneralElection,
                      AnalysisSubject.EntityMunicipalParties,
                      0, o.Code, 0, 0,
                      o.Name,
                      o.Percentage,
                      0, 0, 0,
                      o.TotalVotes))
            .ToListAsync(ct);
}
