using Application.Models;

namespace Application.Repositories;

public interface INotificationRepository
{
    Task<Notification> CreateAsync(int userId, string title, string body);
    Task CreateForUsersAsync(IEnumerable<int> userIds, string title, string body);
    Task<IEnumerable<Notification>> GetByUserAsync(int userId);
    Task MarkAsReadAsync(int notificationId, int userId);
}
