using Application.DTOs;
using Application.Enum;
using Application.Interfaces;
using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DAL.Analysis.Strategies;

public sealed class MayorDetailsStrategy(eZboriDbContext dbContext) : IPartiesResultsStrategy
{
    public AnalysisSubject Subject => AnalysisSubject.MayorDetails;

    public async Task<IEnumerable<PartiesResults>> GetAsync(AnalysisRequest request, CancellationToken ct)
        => await (from o in dbContext.MunicipalityCandidateDetails
                  join m in dbContext.Municipalities on o.MunicipalityCode equals m.Id
                  where o.ElectionYear == request.SelectedYear &&
                        (request.MunicipalityCode == null || o.MunicipalityCode == request.MunicipalityCode)
                  orderby o.TotalVotes descending
                  select new PartiesResults(
                      o.MunicipalityCode,
                      m.Name,
                      o.ElectionYear,
                      ElectionType.LocalElection,
                      AnalysisSubject.MayorDetails,
                      o.AbsenceAndMobileTeamVotes,
                      o.Code,
                      0,
                      o.ConfirmedVotes,
                      o.Name,
                      o.Percentage,
                      o.PostOfficeVotes,
                      o.HaveMandates ? 1 : 0,
                      o.RegularVotes,
                      o.TotalVotes))
            .ToListAsync(ct);
}
