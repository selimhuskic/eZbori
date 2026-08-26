namespace Application.Models.MachineLearning;

public record SearchRecommendationDto(int Id, string Type, int ElectionYear, int Relevance, string Reason = "");
