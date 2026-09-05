using Application.Enum;

namespace Application.Models;

public class ImportJob
{
    public Guid Id { get; set; }
    public int ElectionType { get; set; }
    public short Year { get; set; }
    public ImportJobStatus Status { get; set; } = ImportJobStatus.Queued;
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
