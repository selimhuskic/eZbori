namespace Application.Repositories
{
    public interface IServiceRepository
    {
        Task<IEnumerable<int>> GetLocalElectionYearsAsync();
        Task<IEnumerable<int>> GetGeneralElectionYearsAsync();
    }
}
