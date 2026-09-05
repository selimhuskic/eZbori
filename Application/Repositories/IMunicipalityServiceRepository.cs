using Application.Enum;
using Application.Models;
using Application.ReadModels;

namespace Application.Repositories
{
    public interface IMunicipalityServiceRepository
    {
        Tuple<int, Entity>[] GetAllMunicipalityCodesAndEntity();
        int[] GetAllMunicipalityCodes();
        int[] GetAllMunicipalityCodes(Entity entity);
        int[] GetDistinctStateParliamentElectoralUnits();
        int[] GetDistinctEntityParliamentElectoralUnits(Entity entity);
        int[] GetDistinctCantonCodes();
        int GetCantonCode(int municipalityCode);
        Task<IEnumerable<MunicipalityReadModel>> GetAllMunicipalities();
        Task<Municipality> CreateAsync(Municipality municipality);
        Task UpdateAsync(int id, string name, int population);
        Task DeleteAsync(int id);
    }
}
