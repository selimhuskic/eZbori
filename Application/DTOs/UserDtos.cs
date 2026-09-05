namespace Application.Requests;

public record LoginRequest(
    string Username, 
    string Password);


public record RefreshTokenRequest(
    string RefreshToken);

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Username,
    string DateOfBirth,
    string Password);

public record UpdateRoleRequest(int RoleId);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public record InviteUserRequest(
    string FirstName,
    string LastName,
    string Email,
    int RoleId,
    string? Message);

public record UserProfileDto(
    string Email,
    string UserName,
    string FirstName,
    string LastName,
    DateTime? DateOfBirth,
    string? Role,
    int? MunicipalityId,
    string? MunicipalityName,
    string? ProfileImageBase64);

public record UpdateUserProfileRequest(
    string? Email,
    string? FirstName,
    string? LastName,
    DateTime? DateOfBirth,
    int? MunicipalityId,
    bool? ClearMunicipality,
    string? ProfileImageBase64);

public record ForgotPasswordRequest(string Email);

public record ResetPasswordRequest(string Email, string Token, string NewPassword);

public record AdminUserDto(int Id, string Email, string UserName, string FirstName, string LastName, bool UserVerified, int UserRole);

public record SendNotificationRequest(List<int> UserIds, string Subject, string Body);