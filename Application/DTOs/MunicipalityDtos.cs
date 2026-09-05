using Application.Enum;

namespace Application.DTOs;

public record UpdateMunicipalityRequest(string Name, int Population);

public record CreateMunicipalityRequest(
    int Id,
    string Name,
    Entity Entity,
    int Population,
    StateParliamentElectoralUnit StateParliamentElectoralUnit,
    EntityParliamentElectoralUnit EntityParliamentElectoralUnit,
    CantonParliamentElectoralUnit? CantonParliamentElectoralUnit);
