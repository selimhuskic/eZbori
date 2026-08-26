namespace DAL.Repositories;

public class MunicipalityServiceRepository(eZboriDbContext dboContext) : IMunicipalityServiceRepository
{
    private readonly eZboriDbContext _dbContext = dboContext;

    public Tuple<int, Entity>[] GetAllMunicipalityCodesAndEntity()
    {
        return [.. _dbContext.Municipalities.Select(x => new Tuple<int, Entity>(x.Id, x.Entity))];
    }

    public int[] GetAllMunicipalityCodes()
    {
        return [.. _dbContext.Municipalities.Select(x => x.Id)];
    }

    public int[] GetAllMunicipalityCodes(Entity entity)
    {
        return [.. _dbContext.Municipalities
            .Where(x => x.Entity == entity)
            .Select(x => x.Id)];
    }

    public int[] GetDistinctStateParliamentElectoralUnits()
    {
        return [.. _dbContext.Municipalities
            .Select(x => (int)x.StateParliamentElectoralUnit)
            .Distinct()];
    }

    public int[] GetDistinctEntityParliamentElectoralUnits(Entity entity)
    {
        return _dbContext.Municipalities
            .Where(y => y.Entity == entity)
            .Select(x => (int)x.EntityParliamentElectoralUnit)
            .Distinct()
            .ToArray();
    }

    public int[] GetDistinctCantonCodes()
    {
        return [.. _dbContext.Municipalities
            .Where(x => x.CantonParliamentElectoralUnit != null)
            .Select(x => (int)x.CantonParliamentElectoralUnit!)
            .Distinct()];
    }

    public int GetCantonCode(int municipalityCode)
    {
        var municipality = _dbContext.Municipalities
            .First(x => x.Id == municipalityCode);

        if (municipality.CantonParliamentElectoralUnit == null)
            throw new Exception(); //TODO elaborate

        return (int)municipality.CantonParliamentElectoralUnit;
    }

    public async Task<IEnumerable<MunicipalityReadModel>> GetAllMunicipalities()
    {
        var readModels = _dbContext.Municipalities
            .Select(x => new MunicipalityReadModel(x.Id,
            x.Name,
            x.Entity,
            x.StateParliamentElectoralUnit,
            x.EntityParliamentElectoralUnit,
            x.CantonParliamentElectoralUnit,
            x.Lattitude, 
            x.Longittude,
            x.Population,
            x.Area));

        return await readModels.ToArrayAsync();
    }

    public async Task UpdateAsync(int id, string name, int population)
    {
        var municipality = await _dbContext.Municipalities.FindAsync(id);
        if (municipality is null) return;
        municipality.Name = name;
        municipality.Population = population;
        await _dbContext.SaveChangesAsync();
    }
}
