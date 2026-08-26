namespace Application.ReadModels;

public class CandidateReadModel
{
    public string Name { get; }
    public int ConfirmedVotes { get; }
    public decimal Percentage { get; }

    public CandidateReadModel(string name, int confirmedVotes, decimal percentage)
        => (Name, ConfirmedVotes, Percentage) = (name, confirmedVotes, percentage);
}
