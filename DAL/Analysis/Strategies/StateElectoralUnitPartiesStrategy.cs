using Application.DTOs;
using Application.Enum;
using Application.Interfaces;
using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DAL.Analysis.Strategies;

public sealed class StateElectoralUnitPartiesStrategy(eZboriDbContext dbContext) : IPartiesResultsStrategy
{
    public AnalysisSubject Subject => AnalysisSubject.StateElectoralUnitParties;

    public async Task<IEnumerable<PartiesResults>> GetAsync(AnalysisRequest request, CancellationToken ct)
    {
        if (request.ElectoralUnit.HasValue)
            return await dbContext.StateElectoralUnitParty
                .Where(o => o.ElectionYear == request.SelectedYear &&
                            o.ElectoralUnit == request.ElectoralUnit)
                .OrderByDescending(o => o.TotalVotes)
                .Select(o => new PartiesResults(
                    o.ElectoralUnit,
                    ((StateParliamentElectoralUnit)o.ElectoralUnit).ToString(),
                    o.ElectionYear,
                    ElectionType.GeneralElection,
                    AnalysisSubject.StateElectoralUnitParties,
                    o.AbsenceAndMobileTeamVotes,
                    o.Code,
                    o.CompensationMandates,
                    o.ConfirmedVotes,
                    o.PartyName,
                    o.Percentage,
                    o.PostOfficeVotes,
                    o.RegularMandates,
                    o.RegularVotes,
                    o.TotalVotes))
                .ToListAsync(ct);

        var rawGroups = await dbContext.StateElectoralUnitParty
            .Where(o => o.ElectionYear == request.SelectedYear)
            .GroupBy(o => new { o.PartyName, o.Code, o.ElectionYear })
            .Select(g => new
            {
                g.Key.PartyName,
                g.Key.Code,
                g.Key.ElectionYear,
                TotalVotes                = g.Sum(x => x.TotalVotes),
                RegularVotes              = g.Sum(x => x.RegularVotes),
                ConfirmedVotes            = g.Sum(x => x.ConfirmedVotes),
                PostOfficeVotes           = g.Sum(x => x.PostOfficeVotes),
                AbsenceAndMobileTeamVotes = g.Sum(x => x.AbsenceAndMobileTeamVotes),
                RegularMandates           = g.Sum(x => x.RegularMandates),
                CompensationMandates      = g.Sum(x => x.CompensationMandates),
            })
            .ToListAsync(ct);

        var grandTotal = rawGroups.Sum(g => g.TotalVotes);
        return rawGroups
            .OrderByDescending(g => g.TotalVotes)
            .Select(g => new PartiesResults(
                0, "Svi",
                g.ElectionYear,
                ElectionType.GeneralElection,
                AnalysisSubject.StateElectoralUnitParties,
                g.AbsenceAndMobileTeamVotes,
                g.Code,
                g.CompensationMandates,
                g.ConfirmedVotes,
                g.PartyName,
                grandTotal > 0 ? (decimal)g.TotalVotes / grandTotal * 100 : 0,
                g.PostOfficeVotes,
                g.RegularMandates,
                g.RegularVotes,
                g.TotalVotes));
    }
}
