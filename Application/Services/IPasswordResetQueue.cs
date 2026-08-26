using Application.Models;

namespace Application.Services;

public interface IPasswordResetQueue
{
    Task PublishAsync(PasswordResetMessage message);
}
