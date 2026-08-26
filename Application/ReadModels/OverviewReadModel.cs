namespace Application.ReadModels;

public class OverviewReadModel
{
    public int TotalVoters { get; }
    public int VotersVoted { get; }
    public int  VotersNotVoted { get; }
    public decimal PercentageVoted { get; }

    public OverviewReadModel(int totalVoters,
        int votersVoted, int votersNotVoted, decimal percentageVoted) 
        => (TotalVoters, VotersVoted, VotersNotVoted, PercentageVoted)
            = (totalVoters, votersVoted, votersNotVoted, percentageVoted);
}
