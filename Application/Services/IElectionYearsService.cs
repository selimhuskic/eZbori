namespace Application.Services;

public interface IElectionYearsService
{
    int[] GetGeneralElectionYears();
    int[] GetLocalElectionYears();
}
