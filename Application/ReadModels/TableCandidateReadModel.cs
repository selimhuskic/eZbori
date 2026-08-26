namespace Application.ReadModels;

public class TableCandidateReadModel(string name,
    int? numberOfVoters,
    int votersVoted,
    int electionYear,
    Dictionary<string, int> candidateResults)
{
    public string Name { get; } = name;
    public int? NumberOfVoters { get; } = numberOfVoters;
    public int VotersVoted { get; } = votersVoted;
    public int ElectionYear { get; } = electionYear;
    public Dictionary<string, int> CandidateResults { get; } = candidateResults;

    public readonly KeyValuePair<string, int>[] CandidateResultsForPieChart = candidateResults.Select(x => new KeyValuePair<string, int>(x.Key, x.Value)).ToArray();
}
