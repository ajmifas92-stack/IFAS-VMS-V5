namespace IFAS.VMS.Shared.Validation;

public static class InputValidator
{
    public static ValidationResult Required(string? value, string fieldName)
    {
        return string.IsNullOrWhiteSpace(value)
            ? ValidationResult.Invalid($"{fieldName} is required.")
            : ValidationResult.Valid();
    }

    public static ValidationResult Port(int port)
    {
        return port is < 1 or > 65535
            ? ValidationResult.Invalid("Port must be between 1 and 65535.")
            : ValidationResult.Valid();
    }

    public static ValidationResult PositiveLimit(int value, string fieldName)
    {
        return value <= 0
            ? ValidationResult.Invalid($"{fieldName} must be greater than zero.")
            : ValidationResult.Valid();
    }
}
