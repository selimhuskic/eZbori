using Application.Enum;
using Application.Repositories;
using Application.Services;

namespace DAL.MappingServices;

public class DbElectionYearsService(IElectionCycleRepository repository) : IElectionYearsService
{
    public int[] GetGeneralElectionYears()
        => repository.GetAllYearsForType(ElectionType.GeneralElection);

    public int[] GetLocalElectionYears()
        => repository.GetAllYearsForType(ElectionType.LocalElection);
}
