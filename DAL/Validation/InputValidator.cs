using System.Text.RegularExpressions;

namespace DAL.Validation;

public static class InputValidator
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private static readonly Regex UpperRegex = new(@"[A-Z]", RegexOptions.Compiled);
    private static readonly Regex DigitRegex = new(@"[0-9]", RegexOptions.Compiled);
    private static readonly Regex SpecialRegex = new(@"[!@#$&*~.,%^+=?_]", RegexOptions.Compiled);

    public static void EnsureValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
            throw new UserException("Email adresa nije ispravnog formata.");
    }

    public static void EnsureValidPassword(string? password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 6)
            throw new UserException("Lozinka mora imati najmanje 6 znakova.");
        if (!UpperRegex.IsMatch(password))
            throw new UserException("Lozinka mora sadržavati barem jedno veliko slovo.");
        if (!DigitRegex.IsMatch(password))
            throw new UserException("Lozinka mora sadržavati barem jednu cifru.");
        if (!SpecialRegex.IsMatch(password))
            throw new UserException("Lozinka mora sadržavati barem jedan specijalan znak.");
    }

    public static DateTime EnsureValidDateOfBirth(string? raw)
    {
        if (!DateTime.TryParse(raw, System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var dateOfBirth))
            throw new UserException("Datum rođenja nije ispravnog formata.");

        if (dateOfBirth.Year < 1900 || dateOfBirth > DateTime.UtcNow)
            throw new UserException("Datum rođenja nije validan.");

        return dateOfBirth;
    }

    public static void EnsureDefinedEnum<TEnum>(int value, string fieldName) where TEnum : struct, System.Enum
    {
        if (!Enum.IsDefined(typeof(TEnum), value))
            throw new UserException($"Vrijednost polja '{fieldName}' nije važeća.");
    }
}
