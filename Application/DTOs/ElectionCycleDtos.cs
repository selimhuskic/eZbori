namespace Application.DTOs;

public record CreateElectionCycleRequest(
    short Year,
    byte ElectionType,
    string ApiBaseUrl,
    string ResultKey);
