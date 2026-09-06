using Application.Models;

namespace Application.Repositories;

public interface IFaqRepository : IGenericRepository<FaqItem>
{
    Task<IEnumerable<FaqItem>> GetAllOrderedAsync();
}
