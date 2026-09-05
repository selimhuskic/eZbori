using Application.Services;

namespace DAL.MachineLearning;

public class RankingService(
    IPresidencyRepository presidencyRepository,
    IEntityRepository entityRepository,
    IStateRepository stateRepository,
    ICantonRepository cantonRepository,
    IMunicipalityRepository municipalityRepository,
    ISavedSearchRepository savedSearchRepository) : IRankingService
{
    private static readonly Dictionary<string, int> _typeToSubject = new()
    {
        ["StateElectoralUnitOverview"]        = 1,  ["StateElectoralUnitParty"]           = 1,
        ["StateMunicipalOverview"]            = 1,  ["StateMunicipalParty"]               = 1,
        ["PresidencyResults"]                 = 5,  ["PresidencyOverview"]                = 5,
        ["PresidencyMunicipalResults"]        = 5,  ["PresidencyMunicipalOverview"]       = 5,
        ["EntityElectoralUnitOverview"]       = 9,  ["EntityElectoralUnitParty"]          = 9,
        ["EntityMunicipalOverview"]           = 9,  ["EntityMunicipalParty"]              = 9,
        ["EntityPresidentOverview"]           = 9,  ["EntityPresidentMunicipalCandidate"] = 9,
        ["CantonElectoralUnitOverview"]       = 15, ["CantonElectoralUnitParty"]          = 15,
        ["CantonMunicipalOverview"]           = 15, ["CantonMunicipalParty"]              = 15,
        ["MunicipalityCandidateDetails"]      = 20, ["MunicipalityCandidateOverview"]     = 20,
        ["MunicipalityCouncilOverview"]       = 22, ["MunicipalityCouncilParty"]          = 22,
        ["MunicipalityCouncilMinority"]       = 22,
    };

    private static int SubjectKey(string fullTypeName)
    {
        var name = fullTypeName.Split('.').Last();
        return _typeToSubject.TryGetValue(name, out var s) ? s : -1;
    }

    public async Task<IEnumerable<SearchRecommendationDto>> GetSuggestedSearchesRankedAsync(int top, int? userId = null)
    {
        var fromPresidency   = await presidencyRepository.GetSearchRecommendationAsync();
        var fromEntity       = await entityRepository.GetSearchRecommendationAsync();
        var fromState        = await stateRepository.GetSearchRecommendationAsync();
        var fromCanton       = await cantonRepository.GetSearchRecommendationAsync();
        var fromMunicipality = await municipalityRepository.GetSearchRecommendationAsync();

        var allItems = fromPresidency
            .Concat(fromEntity)
            .Concat(fromState)
            .Concat(fromCanton)
            .Concat(fromMunicipality);

        HashSet<(int subject, int year)> savedPairs = [];
        HashSet<int> savedSubjects = [];
        HashSet<int> savedYears = [];

        if (userId.HasValue)
        {
            var saved = await savedSearchRepository.GetByUserIncludingDeletedAsync(userId.Value);
            foreach (var s in saved)
            {
                var year = (int)s.ElectionYear;
                savedYears.Add(year);
                if (s.AnalysisSubject.HasValue)
                {
                    var subj = (int)s.AnalysisSubject.Value;
                    savedPairs.Add((subj, year));
                    savedSubjects.Add(subj);
                }
            }
        }

        return allItems
            .GroupBy(x => (Subject: SubjectKey(x.Type), x.ElectionYear))
            .Select(g =>
            {
                var item = g.OrderByDescending(x => x.Relevance).First();
                var subject = SubjectKey(item.Type);
                var year = item.ElectionYear;

                int boost;
                string reason;

                if (savedPairs.Contains((subject, year)))
                {
                    boost = 3;
                    reason = "Na osnovu spašenih pretraga";
                }
                else if (savedSubjects.Contains(subject))
                {
                    boost = 2;
                    reason = "Na osnovu spašenih pretraga";
                }
                else if (savedYears.Contains(year))
                {
                    boost = 1;
                    reason = ReasonForSubject(subject);
                }
                else
                {
                    boost = 0;
                    reason = ReasonForSubject(subject);
                }

                return item with { Relevance = item.Relevance + boost, Reason = reason };
            })
            .OrderByDescending(x => x.Relevance)
            .Take(top);
    }

    private static string ReasonForSubject(int subject) => subject switch
    {
        1     => "Državni parlament",
        5     => "Predsjednički izbori",
        9     => "Entitetski parlament",
        15    => "Kantonalni parlament",
        20    => "Općinski kandidati",
        22    => "Općinsko vijeće",
        _     => "Opći izbori",
    };
}
