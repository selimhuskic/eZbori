namespace Application.Models;

public record User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Password { get; set; }
    public int UserRole { get; set; }
    public UserRole Role { get; set; }
    public bool UserVerified { get; set; }
    public int? MunicipalityId { get; set; }
    public Municipality? MunicipalityNavigation { get; set; }
    public string? ProfileImageBase64 { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }
    public bool MustChangePassword { get; set; }

    public void ApplyProfileUpdate(string? email, string? firstName, string? lastName,
        DateTime? dateOfBirth, int? municipalityId, bool clearMunicipality,
        string? profileImageBase64)
    {
        if (email is not null) Email = email;
        if (firstName is not null) FirstName = firstName;
        if (lastName is not null) LastName = lastName;
        if (dateOfBirth.HasValue) DateOfBirth = dateOfBirth;
        if (clearMunicipality) MunicipalityId = null;
        else if (municipalityId.HasValue) MunicipalityId = municipalityId;
        if (profileImageBase64 is not null) ProfileImageBase64 = profileImageBase64;
    }

    public User WithRoles(UserRole role)
    {
        return this with
        {
            Role = role
        };
    }

    public User WithHashedPassword(string hashedPassword)
    {
        return this with
        {
            Password = hashedPassword
        };
    }
}
