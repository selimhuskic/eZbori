using Application.Models;

namespace Application.Services;

public interface IImportQueue
{
    Task PublishAsync(ImportJobMessage message);
}
