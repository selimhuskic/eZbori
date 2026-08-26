using Application.DTOs;
using Application.Interfaces;

namespace DAL.Analysis.Strategies;

public sealed class CantonElectoralUnitPartiesStrategy(eZboriDbContext dbContext) : IPartiesResultsStrategy
{
    public AnalysisSubject Subject => AnalysisSubject.CantonElectoralUnitParties;

    public async Task<IEnumerable<PartiesResults>> GetAsync(AnalysisRequest request, CancellationToken ct)
    {
        if (request.ElectoralUnit.HasValue)
            return await dbContext.CantonElectoralUnitParties
                .Where(o => o.ElectionYear == request.SelectedYear &&
                            o.CantonElectoralUnitCode == request.ElectoralUnit)
                .OrderByDescending(o => o.TotalVotes)
                .Select(o => new PartiesResults(
                    o.CantonElectoralUnitCode,
                    ((CantonParliamentElectoralUnit)o.CantonElectoralUnitCode).ToString(),
                    o.ElectionYear,
                    ElectionType.GeneralElection,
                    AnalysisSubject.CantonElectoralUnitParties,
                    o.AbsenceAndMobileTeamVotes,
                    o.Code,
                    0,
                    o.ConfirmedVotes,
                    o.Name,
                    o.Percentage,
                    o.PostOfficeVotes,
                    o.Mandates,
                    o.RegularVotes,
                    o.TotalVotes))
                .ToListAsync(ct);

        var rawGroups = await dbContext.CantonElectoralUnitParties
            .Where(o => o.ElectionYear == request.SelectedYear)
            .GroupBy(o => new { o.Name, o.Code, o.ElectionYear })
            .Select(g => new
            {
                g.Key.Name,
                g.Key.Code,
                g.Key.ElectionYear,
                TotalVotes                = g.Sum(x => x.TotalVotes),
                RegularVotes              = g.Sum(x => x.RegularVotes),
                ConfirmedVotes            = g.Sum(x => x.ConfirmedVotes),
                PostOfficeVotes           = g.Sum(x => x.PostOfficeVotes),
                AbsenceAndMobileTeamVotes = g.Sum(x => x.AbsenceAndMobileTeamVotes),
                Mandates                  = g.Sum(x => x.Mandates),
            })
            .ToListAsync(ct);

        var grandTotal = rawGroups.Sum(g => g.TotalVotes);
        return rawGroups
            .OrderByDescending(g => g.TotalVotes)
            .Select(g => new PartiesResults(
                0, "Svi",
                g.ElectionYear,
                ElectionType.GeneralElection,
                AnalysisSubject.CantonElectoralUnitParties,
                g.AbsenceAndMobileTeamVotes,
                g.Code,
                0,
                g.ConfirmedVotes,
                g.Name,
                grandTotal > 0 ? (decimal)g.TotalVotes / grandTotal * 100 : 0,
                g.PostOfficeVotes,
                g.Mandates,
                g.RegularVotes,
                g.TotalVotes));
    }
}
