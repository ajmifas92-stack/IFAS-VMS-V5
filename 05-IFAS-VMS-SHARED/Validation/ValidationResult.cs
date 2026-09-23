namespace IFAS.VMS.Shared.Validation;

public sealed class ValidationResult
{
    public bool IsValid { get; private set; }
    public List<string> Errors { get; } = [];

    public static ValidationResult Valid()
        => new() { IsValid = true };

    public static ValidationResult Invalid(params string[] errors)
    {
        var result = new ValidationResult { IsValid = false };
        result.Errors.AddRange(errors);
        return result;
    }
}
