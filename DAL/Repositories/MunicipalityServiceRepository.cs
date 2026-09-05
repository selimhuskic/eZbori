using Microsoft.Data.SqlClient;

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
            throw new UserException($"Općina {municipalityCode} nema definiranu kantonalnu izbornu jedinicu.");

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

    public async Task<Municipality> CreateAsync(Municipality municipality)
    {
        var exists = await _dbContext.Municipalities.AnyAsync(x => x.Id == municipality.Id);
        if (exists)
            throw new UserException($"Općina sa šifrom {municipality.Id} već postoji.");

        await _dbContext.Municipalities.AddAsync(municipality);
        await _dbContext.SaveChangesAsync();
        
        return municipality;
    }

    public async Task UpdateAsync(int id, string name, int population)
    {
        var municipality = await _dbContext.Municipalities.FindAsync(id)
            ?? throw new UserException($"Općina sa šifrom {id} ne postoji.");
        municipality.Name = name;
        municipality.Population = population;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var municipality = await _dbContext.Municipalities.FindAsync(id)
            ?? throw new UserException($"Općina sa šifrom {id} ne postoji.");

        _dbContext.Municipalities.Remove(municipality);
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
        {
            throw new UserException("Općina se ne može obrisati jer je povezana s postojećim izbornim podacima.");
        }
    }
}
