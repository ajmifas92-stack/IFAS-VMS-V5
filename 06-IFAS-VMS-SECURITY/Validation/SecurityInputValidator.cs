namespace IFAS.VMS.Security.Validation;

public static class SecurityInputValidator
{
    public static bool IsStrongEnoughPassword(string? password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
            return false;

        var upper = false;
        var lower = false;
        var digit = false;
        var special = false;

        foreach (var c in password)
        {
            upper |= char.IsUpper(c);
            lower |= char.IsLower(c);
            digit |= char.IsDigit(c);
            special |= !char.IsLetterOrDigit(c);
        }

        return upper && lower && digit && special;
    }

    public static bool IsValidJwtSecret(string? secret)
    {
        if (string.IsNullOrWhiteSpace(secret))
            return false;

        return System.Text.Encoding.UTF8.GetByteCount(secret) >= 32;
    }
}
