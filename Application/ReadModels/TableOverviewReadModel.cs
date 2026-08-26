namespace Application.ReadModels;

// TODO rename
public class TableOverviewReadModel
{
    public string UnitName { get; }
    public int ElectionYear { get; }
    public string Entity { get; }
    public int NumberOfVoters { get; }
    public int VotersVoted { get; }
    public int? NumberOfCandidatesOrParties { get; }
    public decimal PercentageTotalVotes { get; }
    public decimal InvalidBlankBallots { get; }
    public decimal InvalidOtherBallots { get; }
    public decimal TotalInvalidBallots { get; }

    public TableOverviewReadModel(string unitName,
        int electionYear,
        string entity,
        int numberOfVoters,
        int votersVoted,
        int? participatingPartiesOrCandidates,
        decimal percentageTotalVotes,
        decimal invalidBlankBallots,
        decimal invalidOtherBallots)
    {
        UnitName = unitName;
        ElectionYear = electionYear;
        Entity = entity;
        NumberOfVoters = numberOfVoters;
        VotersVoted = votersVoted;
        NumberOfCandidatesOrParties = participatingPartiesOrCandidates;
        PercentageTotalVotes = percentageTotalVotes;
        InvalidBlankBallots = invalidBlankBallots;
        InvalidOtherBallots = invalidOtherBallots;
        TotalInvalidBallots = InvalidOtherBallots + InvalidBlankBallots;
    }
}
