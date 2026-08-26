namespace Application.Models;

public record PasswordResetMessage(string FirstName, string Email, string Token);
