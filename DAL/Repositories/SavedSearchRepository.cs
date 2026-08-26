using Application.Models;
using Application.Repositories;
using DAL.Exceptions;

namespace DAL.Repositories;

public class SavedSearchRepository(eZboriDbContext dbContext) : ISavedSearchRepository
{
    private readonly eZboriDbContext _context = dbContext;

    public async Task<IEnumerable<SavedSearch>> GetByUserAsync(int userId)
        => await _context.SavedSearches
            .Where(s => s.UserId == userId && !s.IsDeleted)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

    public async Task<SavedSearch> CreateAsync(SavedSearch search)
    {
        search.CreatedAt = DateTime.UtcNow;
        await _context.SavedSearches.AddAsync(search);
        await _context.SaveChangesAsync();
        return search;
    }

    public async Task SoftDeleteAsync(int id, int userId)
    {
        var search = await _context.SavedSearches.FindAsync(id)
            ?? throw new UserException($"Saved search with id {id} not found.");

        if (search.UserId != userId)
            throw new UserException("Cannot delete another user's saved search.");

        search.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAllAsync(int userId)
    {
        var searches = await _context.SavedSearches
            .Where(s => s.UserId == userId && !s.IsDeleted)
            .ToListAsync();
        searches.ForEach(s => s.IsDeleted = true);
        await _context.SaveChangesAsync();
    }
}
