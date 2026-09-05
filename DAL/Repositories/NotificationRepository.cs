namespace DAL.Repositories;

public class NotificationRepository(eZboriDbContext dbContext) : INotificationRepository
{
    private readonly eZboriDbContext _context = dbContext;

    public async Task<Notification> CreateAsync(int userId, string title, string body)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Body = body,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
        return notification;
    }

    public async Task CreateForUsersAsync(IEnumerable<int> userIds, string title, string body)
    {
        var now = DateTime.UtcNow;
        var notifications = userIds.Select(userId => new Notification
        {
            UserId = userId,
            Title = title,
            Body = body,
            CreatedAt = now,
            IsRead = false
        });
        await _context.Notifications.AddRangeAsync(notifications);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Notification>> GetByUserAsync(int userId)
        => await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

    public async Task MarkAsReadAsync(int notificationId, int userId)
        => await _context.Notifications
            .Where(n => n.Id == notificationId && n.UserId == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
}
