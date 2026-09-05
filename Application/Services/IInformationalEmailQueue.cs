using Application.Models;

namespace Application.Services;

public interface IInformationalEmailQueue
{
    Task PublishAsync(InformationalEmailMessage message);
}
