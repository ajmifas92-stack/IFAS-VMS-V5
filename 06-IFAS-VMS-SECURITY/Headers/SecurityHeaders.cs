namespace IFAS.VMS.Security.Headers;

public static class SecurityHeaders
{
    public const string XContentTypeOptions = "X-Content-Type-Options";
    public const string XXssProtection = "X-XSS-Protection";
    public const string XFrameOptions = "X-Frame-Options";
    public const string ReferrerPolicy = "Referrer-Policy";
    public const string ContentSecurityPolicy = "Content-Security-Policy";

    public static IReadOnlyDictionary<string, string> DefaultHeaders =>
        new Dictionary<string, string>
        {
            [XContentTypeOptions] = "nosniff",
            [XXssProtection] = "0",
            [XFrameOptions] = "DENY",
            [ReferrerPolicy] = "no-referrer",
            [ContentSecurityPolicy] = "default-src 'self'; frame-ancestors 'none';"
        };
}
