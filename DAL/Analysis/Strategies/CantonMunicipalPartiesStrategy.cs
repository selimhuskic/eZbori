using Application.DTOs;
using Application.Interfaces;

namespace DAL.Analysis.Strategies;

public sealed class CantonMunicipalPartiesStrategy(eZboriDbContext dbContext) : IPartiesResultsStrategy
{
    public AnalysisSubject Subject => AnalysisSubject.CantonMunicipalParties;

    public async Task<IEnumerable<PartiesResults>> GetAsync(AnalysisRequest request, CancellationToken ct)
        => await (from o in dbContext.CantonMunicipalParties
                  join m in dbContext.Municipalities on o.MunicipalityCode equals m.Id
                  where o.ElectionYear == request.SelectedYear &&
                        (request.MunicipalityCode == null || o.MunicipalityCode == request.MunicipalityCode)
                  orderby o.TotalVotes descending
                  select new PartiesResults(
                      o.MunicipalityCode,
                      m.Name,
                      o.ElectionYear,
                      ElectionType.GeneralElection,
                      AnalysisSubject.CantonMunicipalParties,
                      0, o.Code, 0, 0,
                      o.Name,
                      o.Percentage,
                      0, 0, 0,
                      o.TotalVotes))
            .ToListAsync(ct);
}
