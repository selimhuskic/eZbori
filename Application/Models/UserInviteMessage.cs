namespace Application.Models;

public record UserInviteMessage(string FirstName, string LastName, string Email, string? CustomMessage, string? TempPassword);
