namespace DAL.Repositories;

public class ElectionCycleRepository(eZboriDbContext dbContext)
    : GenericRepository<ElectionCycle>(dbContext), IElectionCycleRepository
{
    private readonly eZboriDbContext _context = dbContext;

    public int[] GetYearsForType(ElectionType electionType)
        => [.. _context.ElectionCycles
            .Where(c => c.ElectionType == (byte)electionType && c.DataImported)
            .Select(c => (int)c.Year)];

    public int[] GetAllYearsForType(ElectionType electionType)
        => [.. _context.ElectionCycles
            .Where(c => c.ElectionType == (byte)electionType)
            .Select(c => (int)c.Year)];

    public async Task<ElectionCycle> CreateAsync(ElectionCycle cycle)
    {
        await _context.ElectionCycles.AddAsync(cycle);
        await _context.SaveChangesAsync();
        return cycle;
    }

    public async Task DeleteAsync(int id)
    {
        var cycle = await _context.ElectionCycles.FindAsync(id)
            ?? throw new UserException($"Election cycle with id {id} not found.");
        _context.ElectionCycles.Remove(cycle);
        await _context.SaveChangesAsync();
    }

    public async Task<ElectionCycle> GetByYearAndTypeAsync(short year, ElectionType electionType)
        => await _context.ElectionCycles
            .FirstOrDefaultAsync(c => c.Year == year && c.ElectionType == (byte)electionType)
            ?? throw new UserException($"Election cycle {year}/{electionType} not found.");

    public async Task MarkImportedAsync(short year, ElectionType electionType)
    {
        var cycle = await _context.ElectionCycles
            .FirstOrDefaultAsync(c => c.Year == year && c.ElectionType == (byte)electionType);
        if (cycle is not null)
        {
            cycle.DataImported = true;
            await _context.SaveChangesAsync();
        }
    }
}
