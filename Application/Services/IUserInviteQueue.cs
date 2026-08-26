using Application.Models;

namespace Application.Services;

public interface IUserInviteQueue
{
    Task PublishAsync(UserInviteMessage message);
}
