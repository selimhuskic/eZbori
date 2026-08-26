namespace Application.Models.MachineLearning
{
    public record ElectionResultsData(
        int Next, 
        string PartyName, 
        int Year, 
        int ElectoralUnitCode,
        int ElectionResultId,
        int Prev);
}
