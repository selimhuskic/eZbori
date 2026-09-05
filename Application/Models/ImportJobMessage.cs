namespace Application.Models;

public record ImportJobMessage(Guid JobId, int ElectionType, short Year);
