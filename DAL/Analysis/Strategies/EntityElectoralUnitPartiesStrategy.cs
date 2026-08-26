using Application.DTOs;
using Application.Enum;
using Application.Interfaces;
using DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DAL.Analysis.Strategies;

public sealed class EntityElectoralUnitPartiesStrategy(eZboriDbContext dbContext) : IPartiesResultsStrategy
{
    public AnalysisSubject Subject => AnalysisSubject.EntityElectoralUnitParties;

    public async Task<IEnumerable<PartiesResults>> GetAsync(AnalysisRequest request, CancellationToken ct)
    {
        if (request.ElectoralUnit.HasValue && request.ElectoralUnit != 1 && request.ElectoralUnit != 2)
            return await dbContext.EntityElectoralUnitParty
                .Where(o => o.ElectionYear == request.SelectedYear &&
                            o.ElectoralUnitCode == request.ElectoralUnit)
                .OrderByDescending(o => o.TotalVotes)
                .Select(o => new PartiesResults(
                    o.ElectoralUnitCode,
                    ((EntityParliamentElectoralUnit)o.ElectoralUnitCode).ToString(),
                    o.ElectionYear,
                    ElectionType.GeneralElection,
                    AnalysisSubject.EntityElectoralUnitParties,
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

        IQueryable<EntityElectoralUnitParty> query = request.ElectoralUnit == 1
            ? dbContext.EntityElectoralUnitParty
                .Where(o => o.ElectionYear == request.SelectedYear &&
                            o.ElectoralUnitCode >= 401 && o.ElectoralUnitCode <= 499)
            : request.ElectoralUnit == 2
                ? dbContext.EntityElectoralUnitParty
                    .Where(o => o.ElectionYear == request.SelectedYear &&
                                o.ElectoralUnitCode >= 301 && o.ElectoralUnitCode <= 399)
                : dbContext.EntityElectoralUnitParty
                    .Where(o => o.ElectionYear == request.SelectedYear);

        var rawGroups = await query
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
                AnalysisSubject.EntityElectoralUnitParties,
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
