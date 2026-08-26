using Application.DTOs;
using Application.Enum;
using Application.Interfaces;
using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DAL.Analysis.Strategies;

public sealed class PresidencyResultsStrategy(eZboriDbContext dbContext) : IPartiesResultsStrategy
{
    public AnalysisSubject Subject => AnalysisSubject.PresidencyResults;

    public async Task<IEnumerable<PartiesResults>> GetAsync(AnalysisRequest request, CancellationToken ct)
    {
        var query = dbContext.PresidencyResults
            .Where(o => o.ElectionYear == request.SelectedYear);

        if (request.ElectoralUnit.HasValue &&
            Enum.IsDefined(typeof(Constituency), request.ElectoralUnit.Value))
        {
            var constituency = (Constituency)request.ElectoralUnit.Value;
            query = query.Where(o => o.Constituency == constituency);
        }

        return await query
            .OrderByDescending(o => o.TotalVotes)
            .Select(o => new PartiesResults(
                (int)o.Constituency,
                o.Constituency.ToString(),
                o.ElectionYear,
                ElectionType.GeneralElection,
                AnalysisSubject.PresidencyResults,
                o.AbsenceAndMobileTeamVotes,
                o.Code,
                0,
                o.ConfirmedVotes,
                o.CandidateName,
                o.Percentage,
                0,
                o.MandateWon ? 1 : 0,
                o.RegularVotes,
                o.TotalVotes))
            .ToListAsync(ct);
    }
}
