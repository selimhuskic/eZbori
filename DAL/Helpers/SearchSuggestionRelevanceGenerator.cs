namespace DAL.Helpers;

public static class SearchSuggestionRelevanceGenerator
{
    private static readonly Type[] relevanceTypes4 = [
        typeof(PresidencyResults), typeof(EntityElectoralUnitParty), typeof(EntityPresidentOverview),
        typeof(StateElectoralUnitParty)
    ];

    private static readonly Type[] relevanceTypes3 = [
        typeof(PresidencyOverview), typeof(EntityElectoralUnitOverview),
        typeof(StateElectoralUnitOverview), typeof(CantonElectoralUnitParty)
    ];

    private static readonly Type[] relevanceTypes2 = [
        typeof(PresidencyMunicipalResults), typeof(EntityMunicipalParty),
        typeof(StateMunicipalParty), typeof(MunicipalityCandidateDetails), typeof(MunicipalityCouncilParty)
    ];

    private static readonly Type[] relevanceTypes1 = [
        typeof(PresidencyMunicipalOverview), typeof(EntityMunicipalOverview),
        typeof(StateMunicipalOverview), typeof(CantonElectoralUnitOverview),
        typeof(CantonMunicipalOverview), typeof(CantonMunicipalParty),
        typeof(MunicipalityCandidateOverview), typeof(MunicipalityCouncilOverview), typeof(MunicipalityCouncilMinority)
    ];

    public static int GetRelevance(this int electionYear, IEnumerable<int> orderedElectionYears, Type type)
    {
        if (relevanceTypes4.Contains(type) && orderedElectionYears.LastOrDefault() == electionYear)
            return 4;

        if (relevanceTypes3.Contains(type) && orderedElectionYears.LastOrDefault() == electionYear)
            return 3;

        if (relevanceTypes2.Contains(type) && orderedElectionYears.LastOrDefault() == electionYear)
            return 2;

        if (relevanceTypes1.Contains(type) && orderedElectionYears.LastOrDefault() == electionYear)
            return 1;

        return 0;
    }
}
