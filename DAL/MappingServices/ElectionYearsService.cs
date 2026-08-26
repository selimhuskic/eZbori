using Application.Services;

namespace DAL.MappingServices;

public class ElectionYearsService : IElectionYearsService
{
    private readonly int[] _generalElectionYears = [2018, 2022];
    private readonly int[] _localElectionYears = [2016, 2020, 2024];

    public int[] GetGeneralElectionYears() => _generalElectionYears;

    public int[] GetLocalElectionYears() => _localElectionYears;
}
