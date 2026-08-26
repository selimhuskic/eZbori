using Application.DTOs;
using Application.Interfaces;
namespace DAL.Repositories;

public class AnalysisRepository(
    eZboriDbContext dboContext,
    IEnumerable<IPartiesResultsStrategy> strategies) : IAnalysisRepository
{
    private readonly eZboriDbContext _dbContext = dboContext;
    private readonly IReadOnlyDictionary<AnalysisSubject, IPartiesResultsStrategy> _strategies =
        strategies.ToDictionary(s => s.Subject);

    public async Task<IEnumerable<OverviewResults>> GetGeneralOverviewAsync(AnalysisRequest analysisRequest)
    {
        switch (analysisRequest.AnalysisSubject)
        {
            case AnalysisSubject.StateElectoralUnitGeneral:
                return await _dbContext.StateElectoralUnitOverview
                    .Where(o => o.ElectionYear == analysisRequest.SelectedYear &&
                                (analysisRequest.ElectoralUnit == null || o.ElectoralUnit == analysisRequest.ElectoralUnit))
                    .Select(o => new OverviewResults(
                            o.ElectoralUnit,
                            ((StateParliamentElectoralUnit)o.ElectoralUnit).ToString(),
                            o.ElectionYear,
                            ElectionType.GeneralElection,
                            analysisRequest.AnalysisSubject,
                            o.NumberOfVoters,
                            o.RegularVotes,
                            o.TotalMandates,
                            o.TotalVotes,
                            o.TotalNoVotes,
                            o.ValidVotes,
                            o.TotalInvalidVotes,
                            o.InvalidBlankBallots,
                            o.InvalidOthersBallots,
                            o.PercentageTotalVotes,
                            o.PercentageTotalNoVotes,
                            o.CandidatesNumber,
                            o.ProcessedRegularVotes,
                            o.ProcessedValidVotes,
                            o.ProcessedPostOfficeVotes,
                            o.ProcessedAbsenceAndMobileTeamVotes))
                    .ToListAsync();

            case AnalysisSubject.StateMunicipalGeneral:
                return await (from o in _dbContext.StateMunicipalOverview
                              join m in _dbContext.Municipalities on o.MunicipalityCode equals m.Id
                              where o.ElectionYear == analysisRequest.SelectedYear &&
                                    (analysisRequest.MunicipalityCode == null || o.MunicipalityCode == analysisRequest.MunicipalityCode)
                              select new OverviewResults(
                                      0,
                                      m.Name,
                                      o.ElectionYear,
                                      ElectionType.GeneralElection,
                                      analysisRequest.AnalysisSubject,
                                      o.NumberOfVoters,
                                      0,
                                      0,
                                      o.TotalVotes,
                                      o.TotalNoVotes,
                                      o.ValidVotes,
                                      o.TotalInvalidVotes,
                                      o.InvalidBlankBallots,
                                      o.InvalidOthersBallots,
                                      o.PercentageTotalVotes,
                                      o.PercentageTotalNoVotes,
                                      o.NumberOfCandidates,
                                      0.0m,
                                      0.0m,
                                      0.0m,
                                      0.0m))
                              .ToListAsync();

            case AnalysisSubject.PresidencyGeneral:
                // Map constituency (1=Bosniak, 2=Croat, 3=Serb) → entity (1=FBiH, 2=RS)
                int? presidencyEntityFilter = analysisRequest.ElectoralUnit switch {
                    1 or 2 => (int)Entity.Federation,
                    3      => (int)Entity.RS,
                    _      => null
                };
                return await _dbContext.PresidencyOverview
                    .Where(o => o.ElectionYear == analysisRequest.SelectedYear &&
                                (presidencyEntityFilter == null || (int)o.Entity == presidencyEntityFilter))
                    .Select(o => new OverviewResults(
                            (int)o.Entity,
                            o.Entity.ToString(),
                            o.ElectionYear,
                            ElectionType.GeneralElection,
                            analysisRequest.AnalysisSubject,
                            o.TotalVoters,
                            o.RegularVotes,
                            0,
                            o.TotalVotes,
                            o.TotalNoVotes,
                            o.ValidVotes,
                            o.TotalInvalidVotes,
                            o.InvalidBlankBallots,
                            o.InvalidOthersBallots,
                            o.PercentageTotalVotes,
                            o.PercentageTotalNoVotes,
                            o.CandidatesNumber,
                            o.ProcessedRegularVotes,
                            0.0m,
                            o.ProcessedPostOfficeVotes,
                            o.ProcessedAbsenceAndMobileTeamVotes))
                    .ToListAsync();

            case AnalysisSubject.PresidencyMunicipalGeneral:
                return await (from o in _dbContext.PresidencyMunicipalOverview
                              join m in _dbContext.Municipalities on o.MunicipalityCode equals m.Id
                              where o.ElectionYear == analysisRequest.SelectedYear &&
                                    (analysisRequest.MunicipalityCode == null || o.MunicipalityCode == analysisRequest.MunicipalityCode)
                              select new OverviewResults(
                                      0,
                                      m.Name,
                                      o.ElectionYear,
                                      ElectionType.GeneralElection,
                                      analysisRequest.AnalysisSubject,
                                      o.TotalVoters,
                                      0,
                                      0,
                                      o.TotalVotes,
                                      o.TotalNoVotes,
                                      o.ValidVotes,
                                      o.TotalInvalidVotes,
                                      o.InvalidBlankBallots,
                                      o.InvalidOthersBallots,
                                      o.PercentageTotalVotes,
                                      o.PercentageTotalNoVotes,
                                      o.CandidatesNumber,
                                      0.0m,
                                      0.0m,
                                      0.0m,
                                      0.0m))
                              .ToListAsync();

            case AnalysisSubject.EntityElectoralUnitGeneral:
                // Sentinels 1=FBiH (codes 401-412), 2=RS (codes 301-309); ≥300 = specific unit
                IQueryable<EntityElectoralUnitOverview> euQuery;
                if (analysisRequest.ElectoralUnit == 1)
                    euQuery = _dbContext.EntityElectoralUnitOverview
                        .Where(o => o.ElectionYear == analysisRequest.SelectedYear &&
                                    o.ElectoralUnitCode >= 401 && o.ElectoralUnitCode <= 499);
                else if (analysisRequest.ElectoralUnit == 2)
                    euQuery = _dbContext.EntityElectoralUnitOverview
                        .Where(o => o.ElectionYear == analysisRequest.SelectedYear &&
                                    o.ElectoralUnitCode >= 301 && o.ElectoralUnitCode <= 399);
                else
                    euQuery = _dbContext.EntityElectoralUnitOverview
                        .Where(o => o.ElectionYear == analysisRequest.SelectedYear &&
                                    (analysisRequest.ElectoralUnit == null || o.ElectoralUnitCode == analysisRequest.ElectoralUnit));
                return await euQuery
                    .Select(o => new OverviewResults(
                            o.ElectoralUnitCode,
                            ((EntityParliamentElectoralUnit)o.ElectoralUnitCode).ToString(),
                            o.ElectionYear,
                            ElectionType.GeneralElection,
                            analysisRequest.AnalysisSubject,
                            o.NumberOfVoters,
                            o.RegularVotes,
                            o.TotalMandates,
                            o.TotalVotes,
                            o.TotalNoVotes,
                            o.ValidVotes,
                            o.TotalInvalidVotes,
                            o.InvalidBlankBallots,
                            o.InvalidOthersBallots,
                            o.PercentageTotalVotes,
                            o.PercentageTotalNoVotes,
                            o.NumberOfCandidates,
                            o.ProcessedRegularVotes,
                            o.ProcessedValidVotes,
                            o.ProcessedPostOfficeVotes,
                            o.ProcessedAbsenceAndMobileTeamVotes))
                    .ToListAsync();

            case AnalysisSubject.EntityMunicipalGeneral:
                return await (from o in _dbContext.EntityMunicipalOverview
                              join m in _dbContext.Municipalities on o.MunicipalityCode equals m.Id
                              where o.ElectionYear == analysisRequest.SelectedYear &&
                                    (analysisRequest.MunicipalityCode == null || o.MunicipalityCode == analysisRequest.MunicipalityCode)
                              select new OverviewResults(
                                      0,
                                      m.Name,
                                      o.ElectionYear,
                                      ElectionType.GeneralElection,
                                      analysisRequest.AnalysisSubject,
                                      o.NumberOfVoters,
                                      0,
                                      0,
                                      o.TotalVotes,
                                      o.TotalNoVotes,
                                      o.ValidVotes,
                                      o.TotalInvalidVotes,
                                      o.InvalidBlankBallots,
                                      o.InvalidOthersBallots,
                                      o.PercentageTotalVotes,
                                      o.PercentageTotalNoVotes,
                                      o.NumberOfCandidates,
                                      0.0m,
                                      0.0m,
                                      0.0m,
                                      0.0m))
                              .ToListAsync();

            case AnalysisSubject.EntityPresidentGeneral:
                return await _dbContext.EntityPresidentOverview
                    .Where(o => o.ElectionYear == analysisRequest.SelectedYear)
                    .Select(o => new OverviewResults(
                            (int)o.Entity,
                            o.Entity.ToString(),
                            o.ElectionYear,
                            ElectionType.GeneralElection,
                            analysisRequest.AnalysisSubject,
                            o.NumberOfVoters,
                            o.RegularVotes,
                            0,
                            o.TotalVotes,
                            o.TotalNoVotes,
                            o.ValidVotes,
                            o.TotalInvalidVotes,
                            o.InvalidBlankBallots,
                            o.InvalidOthersBallots,
                            o.PercentageTotalVotes,
                            o.PercentageTotalNoVotes,
                            o.NumberOfCandidates,
                            o.ProcessedRegularVotes,
                            o.ProcessedValidVotes,
                            o.ProcessedPostOfficeVotes,
                            o.ProcessedAbsenceAndMobileTeamVotes))
                    .ToListAsync();

            case AnalysisSubject.CantonElectoralUnitGeneral:
                return await _dbContext.CantonElectoralUnitOverview
                    .Where(o => o.ElectionYear == analysisRequest.SelectedYear &&
                                (analysisRequest.ElectoralUnit == null || o.CantonElectoralUnitCode == analysisRequest.ElectoralUnit))
                    .Select(o => new OverviewResults(
                            o.CantonElectoralUnitCode,
                            ((CantonParliamentElectoralUnit)o.CantonElectoralUnitCode).ToString(),
                            o.ElectionYear,
                            ElectionType.GeneralElection,
                            analysisRequest.AnalysisSubject,
                            o.NumberOfVoters,
                            o.RegularVotes,
                            o.TotalMandates,
                            o.TotalVotes,
                            o.TotalNoVotes,
                            o.ValidVotes,
                            o.TotalInvalidVotes,
                            o.InvalidBlankBallots,
                            o.InvalidOthersBallots,
                            o.PercentageTotalVotes,
                            o.PercentageTotalNoVotes,
                            o.NumberOfCandidates,
                            o.ProcessedRegularVotes,
                            o.ProcessedValidVotes,
                            o.ProcessedPostOfficeVotes,
                            o.ProcessedAbsenceAndMobileTeamVotes))
                    .ToListAsync();

            case AnalysisSubject.CantonMunicipalGeneral:
                return await (from o in _dbContext.CantonMunicipalOverview
                              join m in _dbContext.Municipalities on o.MunicipalityCode equals m.Id
                              where o.ElectionYear == analysisRequest.SelectedYear &&
                                    (analysisRequest.MunicipalityCode == null || o.MunicipalityCode == analysisRequest.MunicipalityCode)
                              select new OverviewResults(
                                      0,
                                      m.Name,
                                      o.ElectionYear,
                                      ElectionType.GeneralElection,
                                      analysisRequest.AnalysisSubject,
                                      o.NumberOfVoters,
                                      0,
                                      0,
                                      o.TotalVotes,
                                      o.TotalNoVotes,
                                      o.ValidVotes,
                                      o.TotalInvalidVotes,
                                      o.InvalidBlankBallots,
                                      o.InvalidOthersBallots,
                                      o.PercentageTotalVotes,
                                      o.PercentageTotalNoVotes,
                                      o.NumberOfCandidates,
                                      0.0m,
                                      0.0m,
                                      0.0m,
                                      0.0m))
                              .ToListAsync();

            default:
                return [];
        }
        
    }   

    public async Task<IEnumerable<OverviewResults>> GetLocalOverviewAsync(AnalysisRequest analysisRequest)
    {
        switch (analysisRequest.AnalysisSubject)
        {
            case AnalysisSubject.MayorGeneral:
                return await (from o in _dbContext.MunicipalityCandidateOverview
                              join m in _dbContext.Municipalities on o.MunicipalityCode equals m.Id
                              where o.ElectionYear == analysisRequest.SelectedYear &&
                                    (analysisRequest.MunicipalityCode == null || o.MunicipalityCode == analysisRequest.MunicipalityCode)
                              select new OverviewResults(
                                      o.MunicipalityCode,
                                      m.Name,
                                      o.ElectionYear,
                                      ElectionType.LocalElection,
                                      analysisRequest.AnalysisSubject,
                                      o.NumberOfVoters,
                                      o.RegularVotes,
                                      0,
                                      o.TotalVotes,
                                      o.TotalNoVotes,
                                      o.ValidVotes,
                                      o.TotalInvalidVotes,
                                      o.InvalidBlankBallots,
                                      o.InvalidOtherBallots,
                                      o.PercentageTotalVotes,
                                      o.PercentageTotalNoVotes,
                                      o.NumberOfCandidates,
                                      o.ProcessedRegularVotes,
                                      o.ProcessedValidVotes,
                                      o.ProcessedPostOfficeVotes,
                                      o.ProcessedAbsenceAndMobileTeamVotes))
                 .ToListAsync();

                case AnalysisSubject.MunicipalCouncilGeneral:
                return await (from o in _dbContext.MunicipalityCouncilOverview
                              join m in _dbContext.Municipalities on o.MunicipalityCode equals m.Id
                              where o.ElectionYear == analysisRequest.SelectedYear &&
                                    (analysisRequest.MunicipalityCode == null || o.MunicipalityCode == analysisRequest.MunicipalityCode)
                              select new OverviewResults(
                                      o.MunicipalityCode,
                                      m.Name,
                                      o.ElectionYear,
                                      ElectionType.LocalElection,
                                      analysisRequest.AnalysisSubject,
                                      o.NumberOfVoters,
                                      o.RegularVotes,
                                      0,
                                      o.TotalVotes,
                                      o.TotalNoVotes,
                                      o.ValidVotes,
                                      o.TotalInvalidVotes,
                                      o.InvalidBlankBallots,
                                      0,
                                      o.PercentageTotalVotes,
                                      o.PercentageTotalNoVotes,
                                      0,
                                      o.ProcessedRegularVotes,
                                      o.ProcessedValidVotes,
                                      o.ProcessedPostOfficeVotes,
                                      o.ProcessedAbsenceAndMobileTeamVotes))
             .ToListAsync();


            default:
                return [];
        }
    }

    public async Task<IEnumerable<PartiesResults>> GetPartiesAsync(AnalysisRequest analysisRequest, CancellationToken ct)
    {
        if (!_strategies.TryGetValue(analysisRequest.AnalysisSubject, out var strategy))
            return [];

        return await strategy.GetAsync(analysisRequest, ct);
    }
}
