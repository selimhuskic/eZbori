namespace DAL.Repositories;

public class FaqRepository(eZboriDbContext dbContext) : GenericRepository<FaqItem>(dbContext), IFaqRepository
{
    private readonly eZboriDbContext _context = dbContext;

    public async Task<IEnumerable<FaqItem>> GetAllOrderedAsync()
        => await _context.FaqItems.OrderBy(x => x.SortOrder).ToListAsync();
}
