using Application.Enum;

namespace Application.DTOs;

public record ImportRequest(ElectionType ElectionType, short Year);
